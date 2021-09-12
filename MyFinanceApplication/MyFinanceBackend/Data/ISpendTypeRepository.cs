using System.Collections.Generic;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Data
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
