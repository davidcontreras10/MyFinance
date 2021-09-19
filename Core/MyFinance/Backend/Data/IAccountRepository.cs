using System;
using System.Collections.Generic;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Data
{
    public interface IAccountRepository
    {
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