using System.Collections.Generic;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Data
{
	public interface ISpendTypeRepository
	{
		IEnumerable<int> DeleteSpendTypeUser(string userId, int spendTypeId);
		IEnumerable<int> AddSpendTypeUser(string userId, int spendTypeId);
		IEnumerable<SpendTypeViewModel> GetSpendTypeByAccountViewModels(string userId, int? accountId);
		IEnumerable<SpendTypeViewModel> GetSpendTypes(string userId, bool includeAll = true);
		IEnumerable<int> AddEditSpendTypes(string userId, ClientSpendType clientSpendType);
	    void DeleteSpendType(string userId, int spendTypeId);
	}
}
