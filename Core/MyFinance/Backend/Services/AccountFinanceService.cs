using System;
using System.Collections.Generic;
using System.Linq;
using MyFinance.Backend.Data;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Services
{
	public class AccountFinanceService : IAccountFinanceService
	{
		private readonly ISpendsRepository _spendsRepository;
        private readonly IAccountRepository _accountRepository;

		public AccountFinanceService(ISpendsRepository spendsRepository, IAccountRepository accountRepository)
		{
			_spendsRepository = spendsRepository;
            _accountRepository = accountRepository;
        }

		#region Publics

		public IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel(IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId)
		{
			return _spendsRepository.GetAccountFinanceViewModel(requestItems, userId);
		}

        public IEnumerable<BankAccountSummary> GetAccountFinanceSummaryViewModel(string userId)
        {
	        var bankAccounts = _accountRepository.GetBankSummaryAccountsPeriodByUserId(userId);
            if (bankAccounts == null || !bankAccounts.Any())
            {
                return new BankAccountSummary[0];
            }

            var requestItems = bankAccounts.Select(acc => CreateBankAccountClientAccountFinanceRequest(acc.AccountPeriodId));
            var financeInfoAccounts = GetAccountFinanceViewModel(requestItems, userId);
	        return financeInfoAccounts.Select(CreateBankAccountSummary);    
        }

        #endregion

        #region Privates

		private static BankAccountSummary CreateBankAccountSummary(AccountFinanceViewModel accountFinanceViewModel)
		{
			if (accountFinanceViewModel == null)
			{
				throw new ArgumentNullException(nameof(accountFinanceViewModel));
			}

			return new BankAccountSummary
			{
				AccountId = accountFinanceViewModel.AccountId,
				AccountName = accountFinanceViewModel.AccountName,
				Balance = new CurrencyAmount
				{
					Amount = accountFinanceViewModel.GeneralBalanceToday,
					CurrencySymbol = accountFinanceViewModel.CurrencySymbol
				}
			};
		}

        private static ClientAccountFinanceViewModel CreateBankAccountClientAccountFinanceRequest(int accountPeriodId)
        {
            return new ClientAccountFinanceViewModel
            {
                AccountPeriodId = accountPeriodId,
                AmountTypeId = 0,
                LoanSpends = true,
                PendingSpends = false
            };
        }

        #endregion
    }
}