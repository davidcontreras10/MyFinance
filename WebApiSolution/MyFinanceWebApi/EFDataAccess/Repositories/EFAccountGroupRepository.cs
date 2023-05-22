using EFDataAccess.Models;
using MyFinanceBackend.Data;
using MyFinanceBackend.Models;
using MyFinanceModel.ClientViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFDataAccess.Repositories
{
	public class EFAccountGroupRepository : IAccountGroupRepository
	{
		private readonly MyFinanceContext _context;

		public EFAccountGroupRepository(MyFinanceContext context)
		{
			_context = context;
		}

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
			var dbItems = accountGroupIds != null && accountGroupIds.Any()
				? _context.AccountGroup.Where(accgp => accgp.UserId.ToString() == userId && accountGroupIds.Any(id => id == accgp.AccountGroupId))
				: _context.AccountGroup.Where(accgp => accgp.UserId.ToString() == userId);
			return dbItems.Select(x => new AccountGroupDetailResultSet
			{
				AccountGroupDisplayValue = x.DisplayValue,
				AccountGroupId = x.AccountGroupId,
				AccountGroupName = x.AccountGroupName,
				AccountGroupPosition = x.AccountGroupPosition ?? 0,
				DisplayDefault = x.DisplayDefault ?? false
			});
		}
	}
}
