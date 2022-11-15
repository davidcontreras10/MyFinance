using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Data
{
    public interface IAccountRepository
    {
	    Task<AccountPeriodBasicInfo> GetAccountPeriodInfoByAccountIdDateTimeAsync(int accountId, DateTime dateTime);

		IEnumerable<AccountPeriodBasicId> GetBankSummaryAccountsPeriodByUserId(string userId);
		IEnumerable<AccountBasicInfo> GetBankSummaryAccountsByUserId(string userId);
        IEnumerable<AccountViewModel> GetOrderedAccountViewModelList(IEnumerable<int> accountIds, string userId);
        IEnumerable<AccountPeriodBasicInfo> GetAccountPeriodBasicInfo(IEnumerable<int> accountPeriodIds);
        AccountPeriodBasicInfo GetAccountPeriodInfoByAccountIdDateTime(int accountId, DateTime dateTime);
        IEnumerable<AccountBasicPeriodInfo> GetAccountBasicInfoByAccountId(IEnumerable<int> accountIds);
        AccountMainViewModel GetAccountDetailsViewModel(string userId, int? accountGroupId);
	    UserAccountsViewModel GetAccountsByUserId(string userId);
	    IEnumerable<SupportedAccountIncludeViewModel> GetSupportedAccountIncludeViewModel(
		    IEnumerable<ClientAddSpendAccountIncludeUpdate> listUpdates, string userId);

	    void DeleteAccount(string userId, int accountId);
	    void AddAccount(string userId, ClientAddAccount clientAddAccount);
	    AddAccountViewModel GetAddAccountViewModel(string userId);
	    IEnumerable<ItemModified> UpdateAccountPositions(string userId,
		    IEnumerable<ClientAccountPosition> accountPositions);

	    void UpdateAccount(string userId, ClientEditAccount clientEditAccount);
	    IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(string userId, int currencyId);
	    IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsViewModel(IEnumerable<int> accountIds, string userId);
    }
}