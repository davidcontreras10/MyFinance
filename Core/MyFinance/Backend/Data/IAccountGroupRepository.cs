using System.Collections.Generic;
using MyFinance.Backend.Models;
using MyFinance.MyFinanceModel.ClientViewModel;

namespace MyFinance.Backend.Data
{
	public interface IAccountGroupRepository
	{
		void DeleteAccountGroup(string userId, int accountGroupId);
		IEnumerable<AccountGroupDetailResultSet> GetAccountGroupDetails(string userId, IEnumerable<int> accountGroupIds = null);
	    int AddorEditAccountGroup(AccountGroupClientViewModel accountGroupClientViewModel);
	}
}
