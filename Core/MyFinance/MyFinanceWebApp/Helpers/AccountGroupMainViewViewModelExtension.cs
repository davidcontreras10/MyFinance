using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.MyFinanceWebApp.Helpers
{
	public static class AccountGroupMainViewViewModelExtension
	{
		private const int AccountsPerRowCount = 2;
		private const int MaxBootstrapColSize = 12;

		public static string GetHtmlId(this AccountGroupMainViewViewModel accountGroupMainViewViewModel, bool definition)
		{
			var id = $"collapse-ag-{accountGroupMainViewViewModel.Id}";
			return definition ? id : $"#{id}";
		}

		public static IEnumerable<AccountsRow> AccountsPerRow(this AccountGroupMainViewViewModel accountGroupMainViewViewModel, string colClassSuffix)
		{
			if (MaxBootstrapColSize % AccountsPerRowCount != 0)
			{
				throw new Exception("Bootstrap sizes are invalid");
			}

			const int colSize = MaxBootstrapColSize / AccountsPerRowCount;
			var colClass = colClassSuffix + colSize;
			var rowsList = new List<AccountsRow>();
			var count = 0;
			var moreAccounts = accountGroupMainViewViewModel.Accounts.Any();
			while (moreAccounts)
			{
				var accountRow = new AccountsRow
				{
					ColSize = colSize,
					BootstrapColSizeClass = colClass
				};
				for (var i = 0; i < AccountsPerRowCount; i++)
				{
					if (count > accountGroupMainViewViewModel.Accounts.Count() - 1)
					{
						moreAccounts = false;
						break;
					}
					accountRow.Add(accountGroupMainViewViewModel.Accounts.ElementAt(count++));
				}

				rowsList.Add(accountRow);
			}

			return rowsList;
		}
	}
}
