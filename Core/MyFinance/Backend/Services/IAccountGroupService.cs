using System.Collections.Generic;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Services
{
	public interface IAccountGroupService
	{
		void DeleteAccountGroup(string userId, int accountGroupId);
	    int AddorEditAccountGroup(AccountGroupClientViewModel accountGroupClientViewModel);
		IEnumerable<AccountGroupDetailViewModel> GetAccountGroupDetailViewModel(string userId, IEnumerable<int> accountGroupIds);
		IEnumerable<AccountGroupViewModel> GetAccountGroupViewModel(string userId);
		IEnumerable<AccountGroupPosition> GetAccountGroupPositions(string userId, bool validateAdd = false,
			int accountGroupIdSelected = 0);

	}
}
