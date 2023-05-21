using MyFinanceBackend.Data;
using MyFinanceBackend.Models;
using MyFinanceModel.ClientViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccess.Repositories
{
	public class EFAccountGroupRepository : IAccountGroupRepository
	{
		public int AddorEditAccountGroup(AccountGroupClientViewModel accountGroupClientViewModel)
		{
			throw new NotImplementedException();
		}

		public void DeleteAccountGroup(string userId, int accountGroupId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<AccountGroupDetailResultSet> GetAccountGroupDetails(string userId, IEnumerable<int> accountGroupIds = null)
		{
			throw new NotImplementedException();
		}
	}
}
