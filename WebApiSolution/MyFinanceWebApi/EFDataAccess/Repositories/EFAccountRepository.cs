using EFDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MyFinanceBackend.Data;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendType = EFDataAccess.Models.SpendType;

namespace EFDataAccess.Repositories
{
    public class EFAccountRepository : BaseEFRepository, IAccountRepository
    {
        public EFAccountRepository(MyFinanceContext context) : base(context)
        {
        }

        public void AddAccount(string userId, ClientAddAccount clientAddAccount)
        {
            var efAccount = new Account
            {
                AccountId = GetNextId(),
                AccountGroupId = clientAddAccount.AccountGroupId,
                Name = clientAddAccount.AccountName,
                PeriodDefinitionId = clientAddAccount.PeriodDefinitionId,
                CurrencyId = clientAddAccount.CurrencyId,
                BaseBudget = clientAddAccount.BaseBudget,
                HeaderColor = CreateFrontStyleDataJson(clientAddAccount.HeaderColor),
                AccountTypeId = (int)clientAddAccount.AccountTypeId,
                DefaultSpendTypeId = clientAddAccount.SpendTypeId,
                FinancialEntityId = clientAddAccount.FinancialEntityId,
                UserId = new Guid(userId)
            };

            Context.Account.Add(efAccount);
            var efAccoountIncludes = clientAddAccount.AccountIncludes != null && clientAddAccount.AccountIncludes.Any()
                ? clientAddAccount.AccountIncludes.Select(acci => new AccountInclude
                {
                    AccountId = acci.AccountId,
                    AccountIncludeId = acci.AccountIncludeId,
                    CurrencyConverterMethodId = acci.CurrencyConverterMethodId
                })
                : null;
            if (efAccoountIncludes != null)
            {
                Context.AddRange(efAccoountIncludes);
            }
        }

        public void DeleteAccount(string userId, int accountId)
        {
            var autoTrxIds = Context.AutomaticTask.Where(at => at.AccountId == accountId).Select(x => x.AutomaticTaskId);
            Context.ExecutedTask.RemoveWhere(x => autoTrxIds.Contains(x.AutomaticTaskId));
            Context.SpInTrxDef.RemoveWhere(x => autoTrxIds.Contains(x.SpInTrxDefId));
            Context.TransferTrxDef.RemoveWhere(x => autoTrxIds.Contains(x.TransferTrxDefId));
            Context.AutomaticTask.RemoveWhere(x => autoTrxIds.Contains(x.AutomaticTaskId));

            Context.UserBankSummaryAccount.RemoveWhere(x => x.AccountId == accountId);

            var accountPeriods = Context.AccountPeriod.Where(x => x.AccountId == accountId);
            Context.SpendOnPeriod.RemoveWhere(x => accountPeriods.Any(accp => accp.AccountPeriodId == x.AccountPeriodId));
            Context.AccountPeriod.RemoveRange(accountPeriods);

            Context.AccountInclude.RemoveWhere(x => x.AccountIncludeId == accountId || x.AccountId == accountId);
            Context.Account.RemoveWhere(x => x.AccountId == accountId);
            Context.SaveChanges();
        }

        public IEnumerable<AccountBasicPeriodInfo> GetAccountBasicInfoByAccountId(IEnumerable<int> accountIds)
        {
            return Context.AccountPeriod.Where(accp => accp.AccountId != null && accountIds.Contains(accp.AccountId.Value)).Include(x => x.Account).Select(accp => new AccountBasicPeriodInfo
            {
                AccountId = accp.AccountId.Value,
                AccountName = accp.Account.Name,
                MaxDate = accp.EndDate.Value,
                MinDate = accp.InitialDate.Value,
            });
        }

        public async Task<IReadOnlyCollection<AccountDetailsPeriodViewModel>> GetAccountDetailsPeriodViewModelAsync(string userId, DateTime dateTime)
        {
            var accounts = await Context.Account.Where(acc => new Guid(userId) == acc.UserId).Include(x => x.AccountPeriod.Where(accp => accp.InitialDate >= dateTime)).ToListAsync();
            return accounts.Select(acc => new AccountDetailsPeriodViewModel
            {
                AccountGroupId = acc.AccountGroupId ?? 0,
                AccountId = acc.AccountId,

            }).ToList();

        }

        public AccountMainViewModel GetAccountDetailsViewModel(string userId, int? accountGroupId)
        {
            var guidUserId = new Guid(userId);
            var accountDetailsViewModels = Context.Account.
                Where(acc =>
                accountGroupId != null
                ? (acc.AccountGroupId == accountGroupId) :
                (guidUserId == acc.UserId)
                ).Select(acc => new AccountDetailsViewModel
                {
                    AccountGroupId = acc.AccountGroupId ?? 0,
                    AccountId = acc.AccountId,
                    AccountName = acc.Name,
                    AccountPosition = acc.Position ?? 0,
                    AccountStyle = CreateFrontStyleData(acc.HeaderColor),
                    BaseBudget = acc.BaseBudget ?? 0,
                    GlobalOrder = acc.Position ?? 0
                });

            var accountGroupViewModels = Context.AccountGroup.Where(
                accg => accountGroupId != null
                    ? (accountGroupId.Value == accg.AccountGroupId)
                    : (guidUserId == accg.UserId.Value)
                ).Select(accg => new AccountGroupViewModel
                {
                    AccountGroupId = accg.AccountGroupId,
                    AccountGroupDisplayValue = accg.DisplayValue,
                    AccountGroupName = accg.AccountGroupName,
                    AccountGroupPosition = accg.AccountGroupPosition ?? 0
                }).OrderBy(accg => accg.AccountGroupPosition);
            return new AccountMainViewModel
            {
                AccountDetailsViewModels = accountDetailsViewModels,
                AccountGroupViewModels = accountGroupViewModels
            };
        }

        public IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsViewModel(IEnumerable<int> accountIds, string userId)
        {
            var userGuid = new Guid(userId);
            var accountTypes = Context.AccountType.Select(acct => new AccountTypeViewModel
            {
                AccountTypeId = acct.AccountTypeId,
                AccountTypeName = acct.AccountTypeName
            });
            var financialEntityViewModels = Context.FinancialEntity.Where(f => !EF.Functions.Like(f.Name, "%default%")).Select(f =>
                new FinancialEntityViewModel
                {
                    FinancialEntityId = f.FinancialEntityId,
                    FinancialEntityName = f.Name
                });

            var currencyConverters = Context.CurrencyConverter.Include(c => c.CurrencyConverterMethod).ThenInclude(x => x.FinancialEntity)
                .ToList();
            var periodTypeViewModels = Context.PeriodDefinition.Include(pd => pd.PeriodType).Where(pd => pd.PeriodType != null).Select(pd =>
            new PeriodTypeViewModel
            {
                CuttingDate = pd.CuttingDate,
                PeriodDefinitionId = pd.PeriodDefinitionId,
                PeriodTypeId = pd.PeriodTypeId,
                PeriodTypeName = pd.PeriodType.Name
            });

            var userAccounts = Context.Account.Where(acc => acc.UserId == userGuid);
            var queryAccounts = Context.Account.Where(acc => accountIds.Contains(acc.AccountId));
            var efCurrencies = Context.Currency.ToList();
            var efAccountGroups = Context.AccountGroup.ToList();
            var acc = queryAccounts.Select(acc => new AccountDetailsInfoViewModel
            {
                AccountName = acc.Name,
                AccountPosition = acc.Position ?? 0,

                SpendTypeViewModels = Context.UserSpendType.Where(ust => ust.UserId == userGuid).Include(x => x.SpendType).Select(x => ToSpendTypeViewModel(x.SpendType, acc.DefaultSpendTypeId)),
                AccountTypeViewModels = accountTypes,
                PeriodTypeViewModels = periodTypeViewModels,
                FinancialEntityViewModels = financialEntityViewModels,
                AccountIncludeViewModels = GetPossibleAccountIncludes(acc, acc.AccountIncludeAccount, userAccounts, currencyConverters),
                CurrencyViewModels = efCurrencies.Select(c => new CurrencyViewModel
                {
                    CurrencyId = c.CurrencyId,
                    CurrencyName = c.Name,
                    Symbol = c.Symbol,
                    Isdefault = c.CurrencyId == acc.CurrencyId
                }),
                AccountGroupViewModels = efAccountGroups.Select(accg => new AccountGroupViewModel
                {
                    AccountGroupDisplayValue = accg.DisplayValue,
                    AccountGroupId = accg.AccountGroupId,
                    AccountGroupName = accg.AccountGroupName,
                    AccountGroupPosition = accg.AccountGroupPosition ?? 0,
                    IsSelected = acc.AccountGroupId == accg.AccountGroupId
                })
            });

            return acc;
        }

        public IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(string userId, int currencyId)
        {
            var userGuid = new Guid(userId);
            var userAccounts = Context.Account.Where(acc => acc.UserId == userGuid);
            var currencyConverterMethods = Context.CurrencyConverterMethod.Include(c => c.CurrencyConverter).Include(c => c.FinancialEntity);
            var applicable = new List<AccountIncludeViewModel>();
            foreach (var account in userAccounts)
            {
                var ccMethods = currencyConverterMethods.Where(ccm =>
                    ccm.CurrencyConverter.CurrencyIdOne == currencyId && ccm.CurrencyConverter.CurrencyIdTwo == account.CurrencyId);
                var acci = new AccountIncludeViewModel
                {
                    AccountId = account.AccountId,
                    AccountName = account.Name,
                    MethodIds = ccMethods.Select(ccm => new MethodId
                    {
                        Id = ccm.CurrencyConverterMethodId,
                        Name = ccm.Name,
                        IsDefault = ccm.IsDefault ?? false
                    })
                };

                applicable.Add(acci);
            }

            return applicable;
        }

        public IEnumerable<AccountPeriodBasicInfo> GetAccountPeriodBasicInfo(IEnumerable<int> accountPeriodIds)
        {
            return Context.AccountPeriod
                .Where(accp => accountPeriodIds.Contains(accp.AccountPeriodId))
                .Include(accp => accp.Account)
                .Select(accp => new AccountPeriodBasicInfo
                {
                    AccountPeriodId = accp.AccountPeriodId,
                    AccountId = accp.AccountId ?? 0,
                    AccountName = accp.Account.Name,
                    InitialDate = accp.InitialDate ?? new DateTime(),
                    EndDate = accp.EndDate ?? new DateTime(),
                    Budget = accp.Budget ?? 0,
                    UserId = accp.Account.UserId.ToString()
                });
        }

        public AccountPeriodBasicInfo GetAccountPeriodInfoByAccountIdDateTime(int accountId, DateTime dateTime)
        {
            var account = Context.Account
                .Where(acc => acc.AccountId == accountId)
                .Include(acc => acc.AccountPeriod
                    .Where(accp => dateTime >= accp.InitialDate && dateTime < accp.EndDate)).FirstOrDefault();
            var accountPeriod = account?.AccountPeriod?.FirstOrDefault();
            if (accountPeriod == null)
            {
                return null;
            }

            return new AccountPeriodBasicInfo
            {
                AccountId = account.AccountId,
                AccountName = account.Name,
                AccountPeriodId = accountPeriod.AccountPeriodId,
                InitialDate = accountPeriod.InitialDate ?? new DateTime(),
                EndDate = accountPeriod.EndDate ?? new DateTime(),
                Budget = accountPeriod.Budget ?? 0,
                UserId = account.UserId.ToString()
            };
        }

        public async Task<AccountPeriodBasicInfo> GetAccountPeriodInfoByAccountIdDateTimeAsync(int accountId, DateTime dateTime)
        {
            var account = await Context.Account
                .Where(acc => acc.AccountId == accountId)
                .Include(acc => acc.AccountPeriod
                    .Where(accp => dateTime >= accp.InitialDate && dateTime < accp.EndDate)).FirstOrDefaultAsync();
            var accountPeriod = account?.AccountPeriod?.FirstOrDefault();
            if (accountPeriod == null)
            {
                return null;
            }

            return new AccountPeriodBasicInfo
            {
                AccountId = account.AccountId,
                AccountName = account.Name,
                AccountPeriodId = accountPeriod.AccountPeriodId,
                InitialDate = accountPeriod.InitialDate ?? new DateTime(),
                EndDate = accountPeriod.EndDate ?? new DateTime(),
                Budget = accountPeriod.Budget ?? 0,
                UserId = account.UserId.ToString()
            };
        }

        public UserAccountsViewModel GetAccountsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public AddAccountViewModel GetAddAccountViewModel(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountBasicInfo> GetBankSummaryAccountsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BankAccountPeriodBasicId> GetBankSummaryAccountsPeriodByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountViewModel> GetOrderedAccountViewModelList(IEnumerable<int> accountIds, string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SupportedAccountIncludeViewModel> GetSupportedAccountIncludeViewModel(IEnumerable<ClientAddSpendAccountIncludeUpdate> listUpdates, string userId)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(string userId, ClientEditAccount clientEditAccount)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemModified> UpdateAccountPositions(string userId, IEnumerable<ClientAccountPosition> accountPositions)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<AccountIncludeViewModel> GetPossibleAccountIncludes(
            Account account,
            IEnumerable<AccountInclude> defaultAccountIncludes,
            IEnumerable<Account> userAccounts,
            IReadOnlyCollection<CurrencyConverter> currencyConverters
            )
        {
            var applicableUserAccounts = userAccounts.Where(acc => acc.AccountId != account.AccountId);
            var accountIncludes = new List<AccountIncludeViewModel>();
            foreach (var appAccount in applicableUserAccounts)
            {
                var appCurrencyConverters = currencyConverters.Where(cc => cc.CurrencyIdOne == account.CurrencyId && cc.CurrencyIdTwo == appAccount.CurrencyId);
                var accountIncludeViewModel = CreateAccountIncludeViewModel(appAccount, appCurrencyConverters, defaultAccountIncludes);
                accountIncludes.Add(accountIncludeViewModel);
            }

            return accountIncludes;
        }

        private static AccountIncludeViewModel CreateAccountIncludeViewModel(
            Account includeAccount
            , IEnumerable<CurrencyConverter> currencyConverters
            , IEnumerable<AccountInclude> defaultAccountIncludes
            )
        {
            var defaultAccountInclude = defaultAccountIncludes.FirstOrDefault(d => d.AccountIncludeId == includeAccount.AccountId);
            var methodIds = new List<MethodId>();
            foreach (var currencyConverter in currencyConverters)
            {
                foreach (var currencyConverterMethod in currencyConverter.CurrencyConverterMethod)
                {
                    methodIds.Add(new MethodId
                    {
                        Id = currencyConverterMethod.CurrencyConverterId,
                        IsDefault = currencyConverterMethod.IsDefault ?? false,
                        Name = currencyConverterMethod.Name,
                        IsSelected = defaultAccountInclude != null &&
                            defaultAccountInclude.CurrencyConverterMethodId == currencyConverterMethod.CurrencyConverterId,
                    });
                }
            }
            return new AccountIncludeViewModel
            {
                AccountId = includeAccount.AccountId,
                AccountName = includeAccount.Name,
                MethodIds = methodIds,
                IsSelected = defaultAccountInclude != null
            };
        }

        private int GetNextId()
        {
            return Context.Account.Max(x => x.AccountId);
        }

        private static SpendTypeViewModel ToSpendTypeViewModel(SpendType spendType, int? defaultSpendTypeId)
        {
            return new SpendTypeViewModel
            {
                Description = spendType.Description,
                IsDefault = defaultSpendTypeId != null && defaultSpendTypeId.Value == spendType.SpendTypeId,
                SpendTypeId = spendType.SpendTypeId,
                SpendTypeName = spendType.Name
            };
        }

        private static FrontStyleData CreateFrontStyleData(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new FrontStyleData();
            }
            json = json.ToUpper();
            try
            {
                var jObject = JObject.Parse(json);
                return new FrontStyleData
                {
                    BorderColor = (string)jObject["borderColor".ToUpper()],
                    HeaderColor = (string)jObject["headerColor".ToUpper()]
                };
            }
            catch (JsonReaderException)
            {
                return new FrontStyleData();
            }
        }

        private static string CreateFrontStyleDataJson(FrontStyleData frontStyleData)
        {
            if (frontStyleData == null)
                return "";

            var result = JsonConvert.SerializeObject(frontStyleData);
            return result;
        }
    }
}
