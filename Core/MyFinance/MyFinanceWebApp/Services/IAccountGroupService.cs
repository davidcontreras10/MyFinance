using System.Collections.Generic;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.MyFinanceWebApp.Services
{
	public interface IAccountGroupService
	{
		void DeleteAccountGroup(string token, int accountGroupId);
        int AddAccountGroup(string token, AccountGroupClientViewModel accountGroupViewModel);
        int EditAccountGroup(string token, AccountGroupClientViewModel accountGroupViewModel);
        AccountGroupDetailViewModel GetAccountGroupDetailViewModel(string token, int accountGroupId);
        IEnumerable<AccountGroupViewModel> GetAccountGroupViewModel(string token);
        IEnumerable<AccountGroupPosition> GetAccountGroupPositions(string token, bool validateAdd = false,
			int accountGroupIdSelected = 0);
	}
}