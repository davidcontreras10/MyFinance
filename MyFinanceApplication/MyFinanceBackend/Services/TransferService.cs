using System.Collections.Generic;
using MyFinanceBackend.ServicesExceptions;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System;
using System.Linq;
using MyFinanceBackend.Data;

namespace MyFinanceBackend.Services
{
	public class TransferService : ITransferService
	{
		#region Attributes

	    private readonly ISpendsRepository _spendsRepository;
		private readonly ISpendsService _spendsService;
		private readonly ICurrencyService _currencyService;
		private readonly ISpendTypeRepository _spendTypeRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly ITransferRepository _transferRepository;

		#endregion

		#region Constructor

		public TransferService(ISpendsService spendsService, ICurrencyService currencyService,
			ISpendTypeRepository spendTypeRepository, IAccountRepository accountRepository,
			ITransferRepository transferRepository, ISpendsRepository spendsRepository)
		{
			_spendsService = spendsService;
			_currencyService = currencyService;
			_spendTypeRepository = spendTypeRepository;
			_accountRepository = accountRepository;
			_transferRepository = transferRepository;
		    _spendsRepository = spendsRepository;
		}

		#endregion

		#region Public methods

		#region Get

		public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId,
			string userId, BalanceTypes balanceType)
		{
			if (balanceType != BalanceTypes.Custom)
			{
				var accountInfo = _spendsRepository.GetAccountFinanceViewModel(accountPeriodId, userId);
				currencyId = accountInfo.CurrencyId;
			}

			var accounts = _transferRepository.GetPossibleDestinationAccount(accountPeriodId, currencyId, userId);
			accounts = GetOrderAccountViewModels(accounts, userId);
			return accounts;
		}

		public IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string userId)
		{
			if (accountId == 0)
				throw new ArgumentException(@"Value cannot be zero", nameof(accountId));

			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));

			var result = _spendsRepository.GetPossibleCurrencies(accountId, userId);
			return result;
		}

		public TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId, string userId)
		{
			var accountInfo = _spendsRepository.GetAccountFinanceViewModel(accountPeriodId, userId);
			if (accountInfo == null)
			{
				return null;
			}

			var accountId = accountInfo.AccountId;
			var currencies = GetPossibleCurrencies(accountId, userId);
			var currencyId = currencies.First(c => c.Isdefault).CurrencyId;
			var accounts = _transferRepository.GetPossibleDestinationAccount(accountPeriodId, currencyId, userId);
			accounts = GetOrderAccountViewModels(accounts, userId);
			var spendTypes = _spendTypeRepository.GetSpendTypeByAccountViewModels(userId, accountId);
			var transferData = CreateAccountFinanceViewModel(accountInfo, currencies, accounts, spendTypes);
			return transferData;
		}

		#endregion

		#region Post

		public IEnumerable<ItemModified> SubmitTransfer(TransferClientViewModel transferClientViewModel)
		{
			if (transferClientViewModel == null)
			{
				throw new ArgumentNullException(nameof(transferClientViewModel));
			}

			if (transferClientViewModel.BalanceType == BalanceTypes.Invalid)
			{
				throw new Exception("Invalid balance type");
			}

			SetTransferClientViewModelAmount(transferClientViewModel);
			if (transferClientViewModel.Amount <= 0)
			{
				throw new Exception("Invalid transfer amount");
			}

			var response = CreateTransferSpendsResponse(transferClientViewModel);
			var itemsModified = new List<SpendItemModified>();
			try
			{
				_transferRepository.BeginTransaction();
				itemsModified.AddRange(_spendsService.AddSpend(response.SpendModel));
				itemsModified.AddRange(_spendsService.AddIncome(response.IncomeModel));
				if (itemsModified.Any())
				{
					var spendIds = itemsModified.GroupBy(i => i.SpendId).Select(gr => gr.First()).Select(i => i.SpendId);
					_transferRepository.AddTransferRecord(spendIds, transferClientViewModel.UserId);
				}
			}
			catch (Exception)
			{
				_transferRepository.RollbackTransaction();
				throw;
			}

			_transferRepository.Commit();
			return itemsModified;
		}

		#endregion

		#endregion

		#region Privates

		private IEnumerable<AccountViewModel> GetOrderAccountViewModels(IEnumerable<AccountViewModel> accountViewModels,
			string userId)
		{
			if (accountViewModels == null || !accountViewModels.Any())
			{
				return new AccountViewModel[] { };
			}

			IEnumerable<AccountViewModel> accountsList = accountViewModels.ToList();
			var orderedAccounts =
				_accountRepository.GetOrderedAccountViewModelList(accountViewModels.Select(acc => acc.AccountId), userId);
			foreach (var orderedAccount in orderedAccounts)
			{
				var account = accountsList.FirstOrDefault(acc => acc.AccountId == orderedAccount.AccountId);
				if (account != null)
				{
					account.GlobalOrder = orderedAccount.GlobalOrder;
				}
			}

			accountsList = accountsList.OrderBy(acc => acc.GlobalOrder);
			return accountsList;
		}

		private TransferSpendsResponse CreateTransferSpendsResponse(TransferClientViewModel transferClientViewModel)
		{
			if (transferClientViewModel == null)
			{
				throw new ArgumentNullException(nameof(transferClientViewModel));
			}

			var originAccountInfo = GetBasicAccountInfo(transferClientViewModel.AccountPeriodId,
				transferClientViewModel.UserId);
			var spend = CreateTransferSpendClientAddSpendModel(transferClientViewModel);
			var income = CreateTransferIncomeClientAddSpendModel(transferClientViewModel, originAccountInfo.AccountId);
			return new TransferSpendsResponse
			{
				SpendModel = spend,
				IncomeModel = income
			};
		}
			
		private ClientAddSpendModel CreateTransferIncomeClientAddSpendModel(
			TransferClientViewModel transferClientViewModel, int originalAccountId)
		{
			if (transferClientViewModel == null)
				throw new ArgumentNullException(nameof(transferClientViewModel));

			var destinationAccountInfo =
				_spendsService.GetAccountsCurrency(new[] {transferClientViewModel.DestinationAccount})
					.First(a => a.AccountId == transferClientViewModel.DestinationAccount);
			var destinationCurrencyConverterMethodId = _transferRepository.GetDefaultCurrencyConvertionMethods(originalAccountId,
				transferClientViewModel.CurrencyId, destinationAccountInfo.CurrencyId,
				transferClientViewModel.UserId);
			var currencyConversionResult = _currencyService.GetExchangeRateResult(destinationCurrencyConverterMethodId,
				transferClientViewModel.SpendDate);
			if (currencyConversionResult == null ||
				currencyConversionResult.ResultTypeValue != ExchangeRateResult.ResultType.Success)
				throw new Exception("Invalid currency conversion method result.");
			var accountCurrencyInfo = _spendsRepository.GetAccountMethodConversionInfo(destinationAccountInfo.AccountId, null,
				transferClientViewModel.UserId, destinationAccountInfo.CurrencyId);
			var includeAccountData = accountCurrencyInfo.Where(a => a.AccountId != destinationAccountInfo.AccountId);
			var originalAccountData = accountCurrencyInfo.FirstOrDefault(a => a.AccountId == destinationAccountInfo.AccountId);
			return new ClientAddSpendModel
			{
				SpendTypeId = transferClientViewModel.SpendTypeId,
				Description = transferClientViewModel.Description,
				Amount = transferClientViewModel.Amount,
				UserId = transferClientViewModel.UserId,
				AmountDenominator = (float) currencyConversionResult.Denominator,
				AmountNumerator = (float) currencyConversionResult.Numerator,
				CurrencyId = destinationAccountInfo.CurrencyId,
				IncludedAccounts = includeAccountData,
				OriginalAccountData = originalAccountData,
				SpendDate = transferClientViewModel.SpendDate,
                IsPending = transferClientViewModel.IsPending
			};
		}

		private ClientAddSpendModel CreateTransferSpendClientAddSpendModel(TransferClientViewModel transferClientViewModel)
		{
		    var clientAddSpendModel =
		        _spendsRepository.CreateClientAddSpendModel(transferClientViewModel, transferClientViewModel.AccountPeriodId);
			return clientAddSpendModel;
		}

		private static TransferAccountDataViewModel CreateAccountFinanceViewModel(
			AccountFinanceViewModel accountFinanceViewModel, IEnumerable<CurrencyViewModel> currencyViewModels,
			IEnumerable<AccountViewModel> accountViewModels, IEnumerable<SpendTypeViewModel> spendTypeViewModels)
		{
			if (accountFinanceViewModel == null)
			{
				throw new ArgumentNullException(nameof(accountFinanceViewModel));
			}

			var transferViewModel = new TransferAccountDataViewModel
			{
				AccountId = accountFinanceViewModel.AccountId,
				AccountName = accountFinanceViewModel.AccountName,
				AccountPeriodId = accountFinanceViewModel.AccountPeriodId,
				Budget = accountFinanceViewModel.Budget,
				CurrencyId = accountFinanceViewModel.CurrencyId,
				CurrencySymbol = accountFinanceViewModel.CurrencySymbol,
				EndDate = accountFinanceViewModel.EndDate,
				GeneralBalance = accountFinanceViewModel.GeneralBalance,
				GeneralBalanceToday = accountFinanceViewModel.GeneralBalanceToday,
				InitialDate = accountFinanceViewModel.InitialDate,
				PeriodBalance = accountFinanceViewModel.PeriodBalance,
				SpendTable = accountFinanceViewModel.SpendTable,
				SpendViewModels = accountFinanceViewModel.SpendViewModels,
				Spent = accountFinanceViewModel.Spent,
				SupportedCurrencies = currencyViewModels,
				SupportedAccounts = accountViewModels,
				SpendTypeViewModels = spendTypeViewModels,
			};

			return transferViewModel;
		}

		private void SetTransferClientViewModelAmount(TransferClientViewModel transferClientViewModel)
		{
			if (transferClientViewModel == null)
			{
				throw new ArgumentNullException(nameof(transferClientViewModel));
			}

			if (transferClientViewModel.BalanceType == BalanceTypes.Custom)
			{
				return;
			}

			var accountInfo = GetBasicAccountInfo(transferClientViewModel.AccountPeriodId,
				transferClientViewModel.UserId);
			if (accountInfo == null)
			{
				throw new DataNotFoundException();
			}

			transferClientViewModel.CurrencyId = accountInfo.CurrencyId;
			transferClientViewModel.Amount = transferClientViewModel.BalanceType == BalanceTypes.AccountPeriodBalance
				? accountInfo.PeriodBalance
				: accountInfo.GeneralBalance;
		}

		#endregion
	}

	internal class TransferSpendsResponse
	{
		public ClientAddSpendModel SpendModel { get; set; }
		public ClientAddSpendModel IncomeModel { get; set; }
	}
}