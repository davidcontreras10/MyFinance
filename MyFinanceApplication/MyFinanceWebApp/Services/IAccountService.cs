using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Services
{
    public interface IAccountService
    {
	    Task<IReadOnlyCollection<AccountDetailsPeriodViewModel>> BasicUserAccountsAsync(string token);

		Task<IEnumerable<BankAccountSummary>> GetBankAccountSummaryAsync(string token);
		Task<IEnumerable<SupportedAccountIncludeViewModel>> GetSupportedAccountIncludeViewModelAsync(
		    IEnumerable<ClientAddSpendAccountIncludeUpdate> listUpdates, string token);
		Task<IEnumerable<AccountFinanceViewModel>> GetSimpleAccountFinanceViewModelAsync(
		    IEnumerable<ClientAccountFinanceViewModel> accountPeriods, string token);
		Task<IEnumerable<AccountFinanceViewModel>> GetAccountFinanceViewModelAsync(IEnumerable<int> accountPeriodIds, bool isPending, string token);
		Task<UserAccountsViewModel> GetAccountsByUserIdAsync(string token);
		AccountMainViewModel GetAccountDetailsViewModel(string token, int accountGroupId);
        IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsInfoViewModel(IEnumerable<int> accountIds, string token);
        IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(string token, int currencyId);
		IEnumerable<ItemModified> UpdateAccountPositions(string token, IEnumerable<ClientAccountPosition> accountPositions);
		void UpdateAccount(string token, ClientEditAccount clientEditAccount);
	    AddAccountViewModel GetAddAccountViewModel(string token);
        void AddAccount(string token, ClientAddAccount clientAddAccount);
        void DeleteAccount(string token, int accountId);
    }
}
