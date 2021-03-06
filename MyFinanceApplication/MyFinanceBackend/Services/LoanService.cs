using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using MyFinanceBackend.Data;
using MyFinanceBackend.ServicesExceptions;
using MyFinanceModel.ViewModel;
using MyFinanceBackend.Utils;

namespace MyFinanceBackend.Services
{
    public interface ILoanService
    {
        IEnumerable<SpendItemModified> CreateLoan(ClientLoanViewModel clientLoanViewModel);
        IEnumerable<SpendItemModified> AddLoanSpend(ClientLoanSpendViewModel clientLoanSpendViewModel);
        AddLoanRecordViewModel GetAddLoanRecordViewModel(DateTime dateTime, int accountId, string userId);
        IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByIds(IEnumerable<int> loanRecordIds);
        IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByCriteriaId(string userId, int loanRecordStatusId, LoanQueryCriteria criteriaId = LoanQueryCriteria.Invalid,
            IEnumerable<int> accountPeriodIds = null, IEnumerable<int> accountIds = null);
        AddLoanSpendViewModel GetAddLoanSpendViewModel(int loanRecordId, string userId);
        IEnumerable<AccountDetailsViewModel> GetSupportedLoanAccount(string userId);
        IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountId, DateTime dateTime,
            string userId, int currencyId);
        IEnumerable<ItemModified> DeleteLoan(int loanRecordId, string userId);
    }

    public class LoanService : ILoanService
    {
        #region Constructor

        public LoanService(ISpendsRepository spendsRepository, ILoanRepository loanRepository, 
            IAccountRepository accountRepository, ITransferService transferService, ICurrencyService currencyService)
        {
            _spendsRepository = spendsRepository;
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
            _transferService = transferService;
            _currencyService = currencyService;
        }

        #endregion

        #region Privates

        private readonly ILoanRepository _loanRepository;
        private readonly ISpendsRepository _spendsRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransferService _transferService;
        private readonly ICurrencyService _currencyService;

        #endregion

        #region Public Methods

        #region Submit

        public IEnumerable<ItemModified> DeleteLoan(int loanRecordId, string userId)
        {
            var result = _loanRepository.DeleteLoan(loanRecordId, userId);
            return result;
        }

        public IEnumerable<SpendItemModified> CreateLoan(ClientLoanViewModel clientLoanViewModel)
        {
            if(clientLoanViewModel == null)
            {
                throw new ArgumentNullException(nameof(clientLoanViewModel));
            }
                        
            var accountPeriod = _accountRepository.GetAccountPeriodInfoByAccountIdDateTime(clientLoanViewModel.AccountId, clientLoanViewModel.SpendDate);
            var destinationAccountPeriod = _accountRepository.GetAccountPeriodInfoByAccountIdDateTime(clientLoanViewModel.DestinationAccountId, clientLoanViewModel.SpendDate);

            var destinationSpendRecord = (ClientBasicAddSpend)clientLoanViewModel.GetCopy();

            destinationSpendRecord.AmountTypeId = TransactionTypeIds.Saving;
            clientLoanViewModel.AmountTypeId = TransactionTypeIds.Spend;
            
            try
            {
                _loanRepository.BeginTransaction();

                var sourceAddSpendResponse = _spendsRepository.AddSpend(clientLoanViewModel, accountPeriod.AccountPeriodId);
                var sourceAddSpendId = sourceAddSpendResponse.First().SpendId;
                var destinationAddSpendResponse = _spendsRepository.AddSpend(destinationSpendRecord, destinationAccountPeriod.AccountPeriodId);
                var destinationAddSpendId = destinationAddSpendResponse.First().SpendId;
                _spendsRepository.AddSpendDependency(sourceAddSpendId, destinationAddSpendId);
                _loanRepository.AddLoanRecord(clientLoanViewModel.UserId, sourceAddSpendId, clientLoanViewModel.LoanName);
                var contcatedModifiedItems = ServicesUtils.SmartConcat(sourceAddSpendResponse, destinationAddSpendResponse);

                _loanRepository.Commit();
                
                return contcatedModifiedItems;
            }
            catch (Exception)
            {
                _loanRepository.RollbackTransaction();
                throw;
            }
        }

        public IEnumerable<SpendItemModified> AddLoanSpend(ClientLoanSpendViewModel clientLoanSpendViewModel)
        {
            if (clientLoanSpendViewModel == null)
            {
                throw new ArgumentNullException(nameof(clientLoanSpendViewModel));
            }

            if (!clientLoanSpendViewModel.FullPayment)
            {
                ValidateLoanPaymentAmount(clientLoanSpendViewModel);
            }
                
            CheckProcessFullPayment(clientLoanSpendViewModel);
            var accountPeriod = _loanRepository.GetAccountPeriodByLoanIdDate(clientLoanSpendViewModel.LoanRecordId,
                clientLoanSpendViewModel.SpendDate);
            if (accountPeriod == null)
            {
                throw new NoPeriodInDateException(clientLoanSpendViewModel.SpendDate);
            }

            var sourceSpend = clientLoanSpendViewModel.GetCopy();
            sourceSpend.AmountTypeId = TransactionTypeIds.Spend;
            clientLoanSpendViewModel.AmountTypeId = TransactionTypeIds.Saving;
            try
            {
                _loanRepository.BeginTransaction();
                var sourceSpendResponse = _spendsRepository.AddSpend(sourceSpend, accountPeriod.AccountPeriodId);
                var sourceSpendId = sourceSpendResponse.First().SpendId;
                var addSpendResponse = _spendsRepository.AddSpend(clientLoanSpendViewModel, accountPeriod.AccountPeriodId).ToList();
                var spendId = addSpendResponse.First().SpendId;
                _spendsRepository.AddSpendDependency(sourceSpendId, spendId);
                _loanRepository.AddLoanSpend(clientLoanSpendViewModel.UserId, spendId, clientLoanSpendViewModel.LoanRecordId);
                if (clientLoanSpendViewModel.FullPayment)
                {
                    _loanRepository.CloseLoan(clientLoanSpendViewModel.LoanRecordId);
                }

                _loanRepository.Commit();
                return addSpendResponse;
            }
            catch (Exception)
            {
                _loanRepository.RollbackTransaction();
                throw;
            }

        }

        #endregion

        #region Get

        public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountId, DateTime dateTime,
            string userId, int currencyId)
        {
            var accountPeriod = _accountRepository.GetAccountPeriodInfoByAccountIdDateTime(accountId, dateTime);
            var result = _transferService.GetPossibleDestinationAccount(accountPeriod.AccountPeriodId, currencyId,
                userId, BalanceTypes.Custom);
            return result;
        }

        public IEnumerable<AccountDetailsViewModel> GetSupportedLoanAccount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var accountMainViewModel = _accountRepository.GetAccountDetailsViewModel(userId, null);
            return accountMainViewModel.AccountDetailsViewModels;
        }

        public IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByIds(IEnumerable<int> loanRecordIds)
        {
            return _loanRepository.GetLoanDetailRecordsByIds(loanRecordIds);
        }

        public AddLoanRecordViewModel GetAddLoanRecordViewModel(DateTime dateTime, int accountId, string userId)
        {
            if (accountId == 0)
            {
                throw new ArgumentException(@"Value cannot be zero", nameof(accountId));
            }

            var accountPeriodInfo = _accountRepository.GetAccountPeriodInfoByAccountIdDateTime(accountId, dateTime);
            var currencies = _spendsRepository.GetPossibleCurrencies(accountPeriodInfo.AccountId, userId);
            var result = new AddLoanRecordViewModel
            {
                PossibleCurrencyViewModels = currencies,
                AccountInfo = accountPeriodInfo
            };

            return result;
        }

        public AddLoanSpendViewModel GetAddLoanSpendViewModel(int loanRecordId, string userId)
        {
            if (loanRecordId == 0)
            {
                throw new ArgumentException(@"Value cannot be zero", nameof(loanRecordId));
            }

            var loanRecord = _loanRepository.GetLoanDetailRecordsByIds(new[] { loanRecordId }).First();
            var accountInfo = _accountRepository.GetAccountBasicInfoByAccountId(new[] {loanRecord.AccountId}).First();
            var currencies = _spendsRepository.GetPossibleCurrencies(accountInfo.AccountId, userId);
            var result = new AddLoanSpendViewModel
            {
                PossibleCurrencyViewModels = currencies,
                AccountInfo = accountInfo,
                LoanRecordId = loanRecordId
            };

            return result;
        }

        public IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByCriteriaId(string userId, int loanRecordStatusId, LoanQueryCriteria criteriaId = LoanQueryCriteria.Invalid,
            IEnumerable<int> accountPeriodIds = null, IEnumerable<int> accountIds = null)
        {
            var loanIds = _loanRepository.GetLoanRecordIdsByCriteria(userId, loanRecordStatusId, criteriaId, accountPeriodIds, accountIds);
            var result = _loanRepository.GetLoanDetailRecordsByIds(loanIds);
            return result;
        }


		#endregion

		#endregion

		#region Private Methods

		private void CheckProcessFullPayment(ClientLoanSpendViewModel clientLoanSpendViewModel)
		{
			if(clientLoanSpendViewModel == null)
			{
				throw new ArgumentNullException(nameof(clientLoanSpendViewModel));
			}

			if (!clientLoanSpendViewModel.FullPayment)
			{
				return;
			}

			var loans = _loanRepository.GetLoanDetailRecordsByIds(new[] { clientLoanSpendViewModel.LoanRecordId });
			if(loans == null || !loans.Any())
			{
				throw new ArgumentException("Loan not found");
			}

			var loanInfo = loans.First();
			clientLoanSpendViewModel.Amount = loanInfo.PaymentPending;
			clientLoanSpendViewModel.CurrencyId = loanInfo.LoanSpendViewModel.AmountCurrencyId;
			clientLoanSpendViewModel.IsPending = false;
		}

        private void ValidateLoanPaymentAmount(ClientLoanSpendViewModel clientLoanSpendViewModel)
        {
            var loanRecord = _loanRepository.GetLoanDetailRecordsByIds(new[] { clientLoanSpendViewModel.LoanRecordId }).FirstOrDefault();
            if(loanRecord == null)
            {
                throw new ArgumentException(nameof(clientLoanSpendViewModel));
            }

            var accountPeriod = _accountRepository.GetAccountPeriodInfoByAccountIdDateTime(loanRecord.AccountId, clientLoanSpendViewModel.SpendDate);
            var loanAccountInfo = _spendsRepository.GetAccountFinanceViewModel(accountPeriod.AccountPeriodId, clientLoanSpendViewModel.UserId);
            double amount;
            if(loanAccountInfo.CurrencyId == clientLoanSpendViewModel.CurrencyId)
            {
                amount = clientLoanSpendViewModel.Amount;
            }
            else
            {
                var clientAddSpendModel = _spendsRepository.CreateClientAddSpendModel(clientLoanSpendViewModel, accountPeriod.AccountPeriodId);
                var conversionResult = _currencyService.GetExchangeRateResult(clientAddSpendModel.OriginalAccountData.ConvertionMethodId, clientLoanSpendViewModel.SpendDate);
                if (!conversionResult.Success)
                {
                    throw new Exception("Invalid conversion result");
                }

                amount = (clientLoanSpendViewModel.Amount * conversionResult.Numerator) / conversionResult.Denominator;
            }

            if(amount > loanRecord.PaymentPending)
            {
                throw new LoanPaymentException(LoanPaymentException.ErrorType.AmountGreaterThanPending);
            }
        }

		#endregion
	}
}