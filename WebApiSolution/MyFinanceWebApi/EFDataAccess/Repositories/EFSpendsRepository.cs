using EFDataAccess.Extensions;
using EFDataAccess.Helpers;
using EFDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MyFinanceBackend.Data;
using MyFinanceBackend.Models;
using MyFinanceBackend.Services;
using MyFinanceBackend.ServicesExceptions;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Spend = EFDataAccess.Models.Spend;

namespace EFDataAccess.Repositories
{
	public class EFSpendsRepository : BaseEFRepository, ISpendsRepository
	{
		private readonly ITrxExchangeService _trxExchangeService;

		public EFSpendsRepository(MyFinanceContext context, ITrxExchangeService trxExchangeService) : base(context)
		{
			_trxExchangeService = trxExchangeService;
		}

		#region Publics

		public async Task<IEnumerable<SpendItemModified>> AddSpendAsync(ClientAddSpendModel clientAddSpendModel)
		{
			if (clientAddSpendModel == null)
				throw new ArgumentNullException(nameof(clientAddSpendModel));
			if (clientAddSpendModel.OriginalAccountData == null)
				throw new ArgumentException(@"OriginalAccountData is null", nameof(clientAddSpendModel));
			await ValidateSpendCurrencyConvertibleValuesAsync(clientAddSpendModel);
			SpendsDataHelper.SetAmountType(clientAddSpendModel, false);

			var setPaymentDate = clientAddSpendModel.IsPending ? null : (DateTime?)clientAddSpendModel.PaymentDate;
			var clientAccountIncluded = await GetConvertedAccountIncludedAsync(clientAddSpendModel);
			var accountIds = clientAccountIncluded.Select(x => x.AccountId);
			var spendDate = clientAddSpendModel.SpendDate;
			var spend = new Models.Spend
			{
				AmountCurrencyId = clientAddSpendModel.CurrencyId,
				AmountTypeId = (int)clientAddSpendModel.AmountTypeId,
				Denominator = clientAddSpendModel.AmountDenominator,
				Description = clientAddSpendModel.Description,
				IsPending = clientAddSpendModel.IsPending,
				Numerator = clientAddSpendModel.AmountNumerator,
				OriginalAmount = clientAddSpendModel.Amount,
				SpendDate = spendDate,
				SetPaymentDate = setPaymentDate,
				SpendTypeId = clientAddSpendModel.SpendTypeId
			};

			await Context.Spend.AddAsync(spend);
			var accountPeriods = await Context.AccountPeriod
				.Where(accp =>
					accountIds.Contains(accp.AccountId ?? 0)
					&& spendDate >= accp.InitialDate && spendDate < accp.EndDate)
				.ToListAsync();
			var modifiedAccs = new List<SpendItemModified>();
			foreach (var accountPeriod in accountPeriods)
			{
				var accountIncluded = clientAccountIncluded.First(acc => acc.AccountId == accountPeriod.AccountId);
				var spendOnPeriod = new SpendOnPeriod
				{
					AccountPeriod = accountPeriod,
					Spend = spend,
					CurrencyConverterMethodId = accountIncluded.CurrencyConverterMethodId,
					Denominator = accountIncluded.Denominator,
					Numerator = accountIncluded.Numerator,
					IsOriginal = accountIncluded.IsOriginal
				};

				modifiedAccs.Add(new SpendItemModified
				{
					AccountId = accountPeriod.AccountPeriodId,
					IsModified = true
				});
				accountPeriod.SpendOnPeriod.Add(spendOnPeriod);
			}

			await Context.SaveChangesAsync();
			modifiedAccs.ForEach(item =>
			{
				item.SpendId = spend.SpendId;
			});

			return modifiedAccs;
		}

		public async Task<IEnumerable<SpendItemModified>> AddSpendAsync(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId)
		{
			var clientAddSpendModel = await CreateClientAddSpendModelAsync(clientBasicAddSpend, accountPeriodId);
			return await AddSpendAsync(clientAddSpendModel);
		}

		public void AddSpendDependency(int spendId, int dependencySpendId)
		{
			throw new NotImplementedException();
		}

		public async Task<ClientAddSpendModel> CreateClientAddSpendModelAsync(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId)
		{
			var accountIds = await Context.AccountPeriod
				.Where(accp => accp.AccountPeriodId == accountPeriodId)
				.Select(accp => accp.AccountId)
				.ToListAsync();
			var accountId = accountIds.First() ?? 0;
			if (accountId == 0)
			{
				throw new Exception($"No account for period {accountPeriodId}");
			}

			var accountCurrencyInfo = await GetAccountMethodConversionInfoAsync(accountId, null,
				clientBasicAddSpend.UserId, clientBasicAddSpend.CurrencyId);
			var originalAccountData = accountCurrencyInfo.FirstOrDefault(a => a.AccountId == accountId);
			var includeAccountData = accountCurrencyInfo.Where(a => a.AccountId != accountId);
			var clientAddSpendModel = new ClientAddSpendModel
			{
				Amount = clientBasicAddSpend.Amount,
				Description = clientBasicAddSpend.Description,
				CurrencyId = clientBasicAddSpend.CurrencyId,
				SpendTypeId = clientBasicAddSpend.SpendTypeId,
				SpendDate = clientBasicAddSpend.SpendDate,
				UserId = clientBasicAddSpend.UserId,
				OriginalAccountData = originalAccountData,
				IncludedAccounts = includeAccountData,
				IsPending = clientBasicAddSpend.IsPending,
				AmountTypeId = clientBasicAddSpend.AmountTypeId,
				AmountType = clientBasicAddSpend.AmountType
			};

			return clientAddSpendModel;
		}

		public async Task<IEnumerable<SpendItemModified>> DeleteSpendAsync(string userId, int spendId)
		{
			try
			{
				await ValidateSpendIdInLoanAsync(spendId);
				var spendsToDelete = new List<Spend>
			{
				await Context.Spend.Where(sp => sp.SpendId == spendId).FirstAsync()
			};

				var spendDependencies = await GetSpendDependenciesAsync(spendId);
				var spendIds = new List<int>()
			{
				spendId
			};

				spendIds.AddRange(spendDependencies.Select(sp => sp.SpendId));
				var affectedAccounts = await Context.SpendOnPeriod.AsNoTracking()
					.Where(sop => spendIds.Contains(sop.SpendId))
					.Include(sop => sop.AccountPeriod)
					.Select(sop => new SpendItemModified
					{
						SpendId = sop.SpendId,
						AccountId = sop.AccountPeriod.AccountId ?? 0,
						IsModified = true
					})
					.ToArrayAsync();
				Context.LoanSpend.RemoveWhere(x => spendIds.Contains(x.SpendId));
				Context.SpendDependencies.RemoveWhere(x => spendIds.Contains(x.SpendId));
				Context.TransferRecord.RemoveWhere(x => spendIds.Contains(x.SpendId));
				Context.SpendOnPeriod.RemoveWhere(x => spendIds.Contains(x.SpendId));
				Context.Spend.RemoveWhere(x => spendIds.Contains(x.SpendId));
				await Context.SaveChangesAsync();
				return affectedAccounts;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
				throw;
			}

		}

		public IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<SpendItemModified>> EditSpendAsync(FinanceSpend financeSpend)
		{
			throw new NotImplementedException();
		}

		public async Task<AccountFinanceViewModel> GetAccountFinanceViewModelAsync(int accountPeriodId, string userId)
		{
			var requestItems = new List<ClientAccountFinanceViewModel>
			{
				new ClientAccountFinanceViewModel
				{
					AccountPeriodId = accountPeriodId,
					LoanSpends = false,
					PendingSpends = true
				}
			};

			var viewModels = await GetAccountFinanceViewModelAsync(requestItems, null);
			return viewModels.First();
		}

		public async Task<IEnumerable<AccountFinanceViewModel>> GetAccountFinanceViewModelAsync(IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId)
		{
			var res = await GetAccountFinanceViewModelAsync(requestItems.ToList(), null);
			return res;
		}

		public async Task<IEnumerable<ClientAddSpendAccount>> GetAccountMethodConversionInfoAsync(int? accountId, int? accountPeriodId, string userId, int currencyId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}

			if (currencyId == 0)
			{
				throw new ArgumentNullException(nameof(currencyId));
			}

			ValidateEitherOrAccountIdValues(accountId, accountPeriodId);
			if (accountId == null || accountId == 0)
			{
				accountId = await Context.AccountPeriod
					.Where(accp => accp.AccountPeriodId == accountPeriodId)
					.Select(accp => accp.AccountId)
					.FirstAsync();
			}

			var account = await Context.Account.AsNoTracking()
				.Where(acc => acc.AccountId == accountId)
				.Include(acc => acc.AccountIncludeAccount)
					.ThenInclude(acci => acci.AccountIncludeNavigation)
				.Include(acc => acc.AccountIncludeAccount)
					.ThenInclude(acci => acci.CurrencyConverterMethod)
						.ThenInclude(acci => acci.CurrencyConverter)
				.FirstAsync();
			var accountIncludeData = account.AccountIncludeAccount;
			var applicableCcms = await Context.CurrencyConverterMethod.AsNoTracking()
				.Include(ccm => ccm.CurrencyConverter)
				.Where(ccm => ccm.CurrencyConverter.CurrencyIdOne == currencyId)
				.ToListAsync();
			var accountIncludeItems = accountIncludeData.Select(acci => new
			{
				DestinationAccount = acci.AccountIncludeNavigation,
				CurrencyConverterMethod = acci.CurrencyConverterMethod
			}).ToList();
			accountIncludeItems.Add(new
			{
				DestinationAccount = account,
				CurrencyConverterMethod = (CurrencyConverterMethod)null
			});

			var clientAddSpendAccounts = new List<ClientAddSpendAccount>();
			foreach (var accItem in accountIncludeItems)
			{
				if (accItem.DestinationAccount.CurrencyId == currencyId)
				{
					var defaultccm = applicableCcms.FirstOrDefault(ccm =>
					ccm.CurrencyConverter.CurrencyIdOne == currencyId
					&& ccm.IsDefault != null && ccm.IsDefault.Value);
					if (defaultccm == null)
					{
						throw new Exception($"Default ccm for {currencyId} does not exist");
					}

					clientAddSpendAccounts.Add(new ClientAddSpendAccount
					{
						AccountId = accItem.DestinationAccount.AccountId,
						ConvertionMethodId = defaultccm.CurrencyConverterId
					});

					continue;
				}

				var currencyConverter = accItem.CurrencyConverterMethod?.CurrencyConverter;
				if (currencyConverter != null
					&& currencyConverter.CurrencyIdOne == currencyId
					&& currencyConverter.CurrencyIdTwo == accItem.DestinationAccount.CurrencyId)
				{
					clientAddSpendAccounts.Add(new ClientAddSpendAccount
					{
						AccountId = accItem.DestinationAccount.AccountId,
						ConvertionMethodId = accItem.CurrencyConverterMethod.CurrencyConverterMethodId
					});

					continue;
				}

				var itemApplicableCcms = applicableCcms.Where(ccm =>
					ccm.CurrencyConverter.CurrencyIdTwo == accItem.DestinationAccount.CurrencyId);
				if (!itemApplicableCcms.Any())
				{
					throw new Exception($"Unable to convert from ${currencyId} to {accItem.DestinationAccount.CurrencyId}");
				}

				var selectedCcm = Validation.IsNotNullOrDefault(accItem.DestinationAccount.FinancialEntityId)
					? applicableCcms.FirstOrDefault(ccm =>
						ccm.FinancialEntityId == accItem.DestinationAccount.FinancialEntityId
						&& ccm.CurrencyConverter.CurrencyIdTwo == accItem.DestinationAccount.CurrencyId)
					: null;
				if (selectedCcm != null)
				{
					clientAddSpendAccounts.Add(new ClientAddSpendAccount
					{
						AccountId = accItem.DestinationAccount.AccountId,
						ConvertionMethodId = selectedCcm.CurrencyConverterMethodId
					});

					continue;
				}

				selectedCcm = applicableCcms.FirstOrDefault(ccm =>
						ccm.CurrencyConverter.CurrencyIdTwo == accItem.DestinationAccount.CurrencyId);
				if (selectedCcm == null)
				{
					throw new Exception($"Unable to convert from ${currencyId} to {accItem.DestinationAccount.CurrencyId}");
				}

				clientAddSpendAccounts.Add(new ClientAddSpendAccount
				{
					AccountId = accItem.DestinationAccount.AccountId,
					ConvertionMethodId = selectedCcm.CurrencyConverterMethodId
				});
			}

			return clientAddSpendAccounts;
		}

		public async Task<IEnumerable<AccountCurrencyPair>> GetAccountsCurrencyAsync(IEnumerable<int> accountIdsArray)
		{
			return await Context.Account
				.Where(acc => accountIdsArray.Contains(acc.AccountId))
				.Select(acc => new AccountCurrencyPair
				{
					AccountId = acc.AccountId,
					CurrencyId = acc.CurrencyId ?? 0
				}).ToListAsync();
		}

		public IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string userId)
		{
			throw new NotImplementedException();
		}

		public DateRange GetDateRange(string accountIds, DateTime? dateTime, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<EditSpendViewModel> GetEditSpendViewModel(int accountPeriodId, int spendId, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SavedSpend> GetSavedSpends(int spendId)
		{
			throw new NotImplementedException();
		}

		public SpendActionAttributes GetSpendAttributes(int spendId)
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction()
		{
			Context.Database.RollbackTransaction();
		}

		public void BeginTransaction()
		{
			Context.Database.BeginTransaction();
		}

		public void Commit()
		{
			Context.Database.CommitTransaction();
		}

		#endregion

		private async Task<IReadOnlyCollection<Spend>> GetSpendDependenciesAsync(int spendId)
		{
			var dependencies = new List<Spend>();
			var trasnferId = await Context.TransferRecord.AsNoTracking()
				.Where(tr => tr.SpendId == spendId)
				.Select(tr => tr.TransferRecordId)
				.FirstOrDefaultAsync();
			if (trasnferId > 0)
			{
				dependencies.AddRange(
					await Context.TransferRecord.AsNoTracking()
						.Where(t => t.TransferRecordId == trasnferId)
						.Select(t => t.Spend)
						.Where(sp => sp.SpendId != spendId)
						.ToListAsync()
					);
			}

			var loanId = await Context.LoanRecord.AsNoTracking()
				.Where(lr => lr.SpendId == spendId)
				.Select(lr => lr.LoanRecordId)
				.FirstOrDefaultAsync();
			if (loanId > 0)
			{
				var loanTrxs = await Context.LoanSpend.AsNoTracking()
					.Where(ls => ls.LoanRecordId == loanId)
					.Include(ls => ls.Spend)
					.Select(ls => ls.Spend)
					.ToListAsync();
				dependencies.AddRange(loanTrxs);
			}

			var allEvaluate = await Context.Spend.AsNoTracking()
				.Where(sp => sp.SpendId == spendId)
				.ToListAsync();
			allEvaluate.AddRange(dependencies);
			dependencies.AddRange(await GetDependenciesRecursivleyAsync(allEvaluate));
			return dependencies;
		}

		private async Task<IReadOnlyCollection<Spend>> GetDependenciesRecursivleyAsync(IReadOnlyCollection<Spend> spends)
		{
			var resultList = new List<Spend>();
			var evaluateList = spends.ToList();
			while (evaluateList.Any())
			{
				var evaluateElement = evaluateList.First();
				evaluateList.Remove(evaluateElement);
				var firstLevelDeps = await GetFirstLevelDependenciesAsync(evaluateElement.SpendId);
				if (firstLevelDeps != null && firstLevelDeps.Any())
				{
					evaluateList.AddRange(firstLevelDeps);
					resultList.AddRange(firstLevelDeps);
				}
			}

			return resultList;
		}

		private async Task<IReadOnlyCollection<Spend>> GetFirstLevelDependenciesAsync(int spendId)
		{
			return await Context.SpendDependencies.AsNoTracking()
				.Where(sd => sd.SpendId == spendId)
				.Include(sd => sd.DependencySpend)
				.Select(sd => sd.DependencySpend)
				.ToListAsync();
		}

		private async Task ValidateSpendIdInLoanAsync(int spendId)
		{
			var existsInLoan = await Context.LoanRecord
				.Where(lr => lr.SpendId == spendId).AnyAsync();
			if (existsInLoan)
			{
				throw new Exception("Not allowed to delete loan record spend");
			}

			if (await Context.SpendDependencies
				.Include(spd => spd.Spend)
					.ThenInclude(sp => sp.LoanRecord)
				.AnyAsync(spd => spd.DependencySpendId == spendId && spd.Spend.LoanRecord != null))
			{
				throw new Exception("Not allowed to delete loan record spend");
			}
		}

		private void ValidateEitherOrAccountIdValues(int? id1, int? id2)
		{

			if ((id1 == null || id1 == 0) && (id2 == null || id2 == 0))
			{
				throw new AggregateException(new ArgumentException(@"Both parameters cannot be empty",
					nameof(id1)));
			}

			if ((id1 != null && id1 != 0) && (id2 != null && id2 != 0))
			{
				throw new AggregateException(new ArgumentException(@"Only one parameters can be specified",
					nameof(id1)));
			}
		}

		private async Task<IReadOnlyCollection<AccountFinanceViewModel>> GetAccountFinanceViewModelAsync(
			IReadOnlyCollection<ClientAccountFinanceViewModel> requestItems,
			DateTime? currentDate
			)
		{
			if (requestItems == null || !requestItems.Any())
			{
				return Array.Empty<AccountFinanceViewModel>();
			}

			var requiresLoan = requestItems.Any(r => r.LoanSpends);
			var accountPeriodIds = requestItems.Select(accp => accp.AccountPeriodId);
			var infoIds = await Context.AccountPeriod.AsNoTracking()
				.Where(acc => accountPeriodIds.Contains(acc.AccountPeriodId))
				.Select(accp => new { accp.AccountId, accp.AccountPeriodId })
				.ToListAsync();
			var accountIds = infoIds.Select(acc => acc.AccountId);
			IQueryable<Models.Account> query = Context.Account.AsNoTracking()
				.Where(acc => accountIds.Contains(acc.AccountId))
				.Include(acc => acc.Currency)
				.Include(acc => acc.AccountPeriod)
					.ThenInclude(accp => accp.SpendOnPeriod)
						.ThenInclude(sop => sop.Spend)
							.ThenInclude(sp => sp.AmountCurrency)
				.Include(acc => acc.AccountPeriod)
					.ThenInclude(accp => accp.SpendOnPeriod)
						.ThenInclude(sop => sop.Spend)
							.ThenInclude(sp => sp.SpendType);
			if (requiresLoan)
			{
				query = query
					.Include(acc => acc.AccountPeriod)
						.ThenInclude(accp => accp.SpendOnPeriod)
							.ThenInclude(sop => sop.Spend)
								.ThenInclude(sp => sp.LoanRecord);
			}

			var accViewModels = new List<AccountFinanceViewModel>();
			var accounts = await query.ToListAsync();
			foreach (var account in accounts)
			{
				var accInfo = infoIds.First(x => x.AccountId == account.AccountId);
				var selectedAccountPeriod = account.AccountPeriod.First(accp => accp.AccountPeriodId == accInfo.AccountPeriodId);
				var requestParams = requestItems.First(r => r.AccountPeriodId == selectedAccountPeriod.AccountPeriodId);

				var currentAccountPeriod = AccountHelpers.GetCurrentAccountPeriod(currentDate, account);
				var periodsSumResult = SumPeriods(account.AccountPeriod, requestParams,
					currentAccountPeriod?.AccountPeriodId, selectedAccountPeriod.AccountPeriodId);
				var periodBudget = selectedAccountPeriod.Budget ?? 0;
				var viewModel = new AccountFinanceViewModel
				{
					AccountId = account.AccountId,
					AccountName = account.Name,
					AccountPeriodId = accInfo.AccountPeriodId,
					Budget = periodBudget,
					CurrencyId = account.CurrencyId ?? 0,
					CurrencySymbol = account.Currency.Symbol,
					EndDate = selectedAccountPeriod.EndDate ?? new DateTime(),
					GlobalOrder = account.Position ?? 0,
					InitialDate = selectedAccountPeriod.InitialDate ?? new DateTime(),
					SpendViewModels = periodsSumResult.SelectedPeriodSum.SpendViewModels,
					Spent = (float)(periodsSumResult.SelectedPeriodSum.BalanceSum),
					GeneralBalance = (float)(periodsSumResult.NotCurrentTotalBudgetSum - periodsSumResult.NotCurrentTotalTrxSum),
					GeneralBalanceToday = (float)(periodsSumResult.CurrentIncludedBudgetSum - periodsSumResult.CurrentIncludedTrxSum),
					PeriodBalance = (float)(periodBudget - periodsSumResult.SelectedPeriodSum.BalanceSum),
				};

				accViewModels.Add(viewModel);
			}

			return accViewModels;
		}

		private static AccountPeriodsSumRes SumPeriods(
			ICollection<Models.AccountPeriod> accountPeriods
			, ClientAccountFinanceViewModel requestParams
			, int? currentPeriodId
			, int selectedPeriodId
			)
		{
			var sumResult = new AccountPeriodsSumRes();
			if (accountPeriods == null || !accountPeriods.Any())
			{
				return sumResult;
			}

			foreach (var accountPeriod in accountPeriods)
			{
				var isCurrentPeriod = accountPeriod.AccountPeriodId == currentPeriodId;
				var isSelectedPeriod = accountPeriod.AccountPeriodId == selectedPeriodId;
				var trxSumResult = GetTrxSumResult(accountPeriod.SpendOnPeriod, requestParams, !isSelectedPeriod);
				if (isSelectedPeriod)
				{
					sumResult.SelectedPeriodSum = trxSumResult;
				}

				sumResult.CurrentIncludedTrxSum += trxSumResult.BalanceSum;
				sumResult.CurrentIncludedBudgetSum += accountPeriod.Budget ?? 0;
				if (!isCurrentPeriod)
				{
					sumResult.NotCurrentTotalTrxSum += trxSumResult.BalanceSum;
					sumResult.NotCurrentTotalBudgetSum += accountPeriod.Budget ?? 0;
				}
			}

			return sumResult;
		}

		private static TrxSumResult GetTrxSumResult(ICollection<SpendOnPeriod> spendOnPeriods, ClientAccountFinanceViewModel requestParams, bool ignoreTrxViewModels)
		{
			var sumResult = new TrxSumResult();
			foreach (var spendOnPeriod in spendOnPeriods.Where(sop => FilterSpend(sop.Spend, requestParams)))
			{
				if (!ignoreTrxViewModels)
				{
					var spendViewModel = spendOnPeriod.ToSpendSpendViewModel();
					sumResult.SpendViewModels.Add(spendViewModel);

				}

				sumResult.BalanceSum += spendOnPeriod.GetAmount();
			}

			return sumResult;
		}

		private static bool FilterSpend(Models.Spend spend, ClientAccountFinanceViewModel request)
		{
			if (!request.PendingSpends && spend.IsPending)
			{
				return false;
			}

			if (!request.LoanSpends && spend.LoanRecord != null)
			{
				return false;
			}

			return request.AmountTypeId == 0 || spend.AmountTypeId == request.AmountTypeId;
		}

		private async Task<IEnumerable<AddSpendAccountDbValues>> GetConvertedAccountIncludedAsync(ISpendCurrencyConvertible spendCurrencyConvertible)
		{
			var accountIds = SpendsDataHelper.GetInvolvedAccountIds(spendCurrencyConvertible);
			var accountCurrencyPairList = await GetAccountsCurrencyAsync(accountIds);
			return await _trxExchangeService.ConvertTrxCurrencyAsync(spendCurrencyConvertible, accountCurrencyPairList.ToList());
		}

		private async Task ValidateSpendCurrencyConvertibleValuesAsync(ISpendCurrencyConvertible spendCurrencyConvertible)
		{
			if (spendCurrencyConvertible == null)
				throw new ArgumentNullException(nameof(spendCurrencyConvertible));
			var accountData =
				spendCurrencyConvertible.IncludedAccounts.Select(
					item => SpendsDataHelper.CreateClientAddSpendCurrencyData(item, spendCurrencyConvertible.CurrencyId));
			var clientAddSpendValidationResultSet = await GetClientAddSpendValidationResultSetAsync(accountData);
			var invalids = clientAddSpendValidationResultSet.Where(item => !item.IsSuccess);
			if (!invalids.Any())
				return;
			var invalidAccounts = invalids.Select(SpendsDataHelper.CreateAccountCurrencyConverterData);
			throw new InvalidAddSpendCurrencyException(invalidAccounts);
		}

		private async Task<IEnumerable<ClientAddSpendValidationResultSet>> GetClientAddSpendValidationResultSetAsync(
			IEnumerable<ClientAddSpendCurrencyData> clientAddSpendCurrencyDataList)
		{
			var accountIds = clientAddSpendCurrencyDataList.Select(c => c.AccountId);
			var accounts = await Context.Account
				.Where(acc => accountIds.Contains(acc.AccountId))
				.ToListAsync();
			var ccmIds = clientAddSpendCurrencyDataList.Select(ccm => ccm.CurrencyConverterMethodId);
			var currencyConverterMethods = await Context.CurrencyConverterMethod
				.Where(ccm => ccmIds.Contains(ccm.CurrencyConverterMethodId))
				.Include(ccm => ccm.CurrencyConverter)
				.ToListAsync();
			var results = new List<ClientAddSpendValidationResultSet>();
			foreach (var cAccount in clientAddSpendCurrencyDataList)
			{
				var account = accounts.First(acc => acc.AccountId == cAccount.AccountId);
				var ccm = currencyConverterMethods.First(x => x.CurrencyConverterMethodId == cAccount.CurrencyConverterMethodId);
				var result = new ClientAddSpendValidationResultSet
				{
					AccountId = account.AccountId,
					AccountName = account.Name,
					AmountCurrencyId = cAccount.AmountCurrencyId,
					ConvertCurrencyId = ccm.CurrencyConverterId,
					CurrencyIdOne = ccm.CurrencyConverter.CurrencyIdOne,
					CurrencyIdTwo = ccm.CurrencyConverter.CurrencyIdTwo,
					IsSuccess = ccm.CurrencyConverter.CurrencyIdOne == cAccount.AmountCurrencyId
						&& ccm.CurrencyConverter.CurrencyIdTwo == account.CurrencyId
				};

				results.Add(result);
			}

			return results;
		}

		private class AccountPeriodsSumRes
		{

			public double NotCurrentTotalBudgetSum { get; set; }
			public double NotCurrentTotalTrxSum { get; set; }
			public double CurrentIncludedTrxSum { get; set; }
			public double CurrentIncludedBudgetSum { get; set; }

			public TrxSumResult SelectedPeriodSum { get; set; } = new TrxSumResult();
		}

		private class TrxSumResult
		{
			public double BalanceSum { get; set; }
			public List<SpendViewModel> SpendViewModels { get; set; } = new List<SpendViewModel>();
		}
	}
}
