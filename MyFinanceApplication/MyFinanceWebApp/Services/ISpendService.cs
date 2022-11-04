using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Services
{
    public interface ISpendService
    {
        IEnumerable<SpendItemModified> ConfirmPendingSpend(int spendId, string token);
        IEnumerable<EditSpendViewModel> GetEditSpendViewModel(IEnumerable<int> accountPeriodIds, string token, int spendId);
        IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string token);

        Task<IEnumerable<AddSpendViewModel>> GetAddSpendViewModelAsync(
	        IEnumerable<int> accountPeriodIds,
	        string token
        );
        IEnumerable<ItemModified> AddSpendCurrency(string token, ClientAddSpendModel clientAddSpendModel);
        IEnumerable<ItemModified> AddIncome(string token, ClientAddSpendModel clientAddSpendModel);
        IEnumerable<ItemModified> DeleteSpend(string token, int spendId);
        IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model, string token);
        AddPeriodData GetAddPeriodData(string token, int accountId, string userId);
        DateRange GetDateRange(string accountIds, DateTime? dateTime, string token, string userId);
		SpendActionResult GetSpendActionResult(int spendId, ResourceActionNames actionType, ApplicationModules applicationModule, string token);
	    Task<IEnumerable<ItemModified>> AddBasicSpendAsync(string token, ClientBasicTrxByPeriod clientBasicTrxByPeriod);
	    Task<IEnumerable<ItemModified>> AddBasicIncomeAsync(string token, ClientBasicTrxByPeriod clientBasicTrxByPeriod);
	}
}
