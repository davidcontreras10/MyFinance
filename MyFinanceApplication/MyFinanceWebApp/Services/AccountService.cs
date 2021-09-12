using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Services
{
    public class AccountService : BaseService, IAccountService
    {
        #region Methods

		public IEnumerable<ItemModified> UpdateAccountPositions(string token, IEnumerable<ClientAccountPosition> accountPositions)
		{
			throw new NotImplementedException();
		}

	    public void UpdateAccount(string token, ClientEditAccount clientEditAccount)
	    {
		    throw new NotImplementedException();
	    }

        public AddAccountViewModel GetAddAccountViewModel(string token)
        {
            throw new NotImplementedException();
        }

        public void AddAccount(string token, ClientAddAccount clientAddAccount)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(string token, int accountId)
        {
            throw new NotImplementedException();
        }

	    public Task<IEnumerable<BankAccountSummary>> GetBankAccountSummaryAsync(string token)
	    {
		    throw new NotImplementedException();
	    }

	    public Task<IEnumerable<SupportedAccountIncludeViewModel>> GetSupportedAccountIncludeViewModelAsync(IEnumerable<ClientAddSpendAccountIncludeUpdate> listUpdates, string token)
	    {
		    throw new NotImplementedException();
	    }

	    public Task<IEnumerable<AccountFinanceViewModel>> GetSimpleAccountFinanceViewModelAsync(IEnumerable<ClientAccountFinanceViewModel> accountPeriods, string token)
	    {
		    throw new NotImplementedException();
	    }

	    public Task<IEnumerable<AccountFinanceViewModel>> GetAccountFinanceViewModelAsync(IEnumerable<int> accountPeriodIds, bool isPending, string token)
	    {
		    throw new NotImplementedException();
	    }

	    public Task<UserAccountsViewModel> GetAccountsByUserIdAsync(string token)
	    {
		    throw new NotImplementedException();
	    }

	    public AccountMainViewModel GetAccountDetailsViewModel(string token, int accountGroupId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsInfoViewModel(IEnumerable<int> accountIds, string token)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(string token, int currencyId)
        {
            throw new NotImplementedException();
        }

        #endregion
	}
}