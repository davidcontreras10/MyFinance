using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceBackend.Models;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
{
    public interface ISpendsService
    {
        Task<IEnumerable<SpendItemModified>> AddIncomeAsync(ClientAddSpendModel clientAddSpendModel);
        Task<IEnumerable<SpendItemModified>> AddSpendAsync(ClientAddSpendModel clientAddSpendModel);
        IEnumerable<SpendItemModified> DeleteSpend(string userId, int spendId);
        DateRange GetDateRange(string accountIds, DateTime? dateTime, string userId);
        IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model);
        IEnumerable<AccountCurrencyPair> GetAccountsCurrency(IEnumerable<int> accountIdsArray);
        IEnumerable<SavedSpend> GetSavedSpends(int spendId);
        IEnumerable<SpendItemModified> ConfirmPendingSpend(int spendId, DateTime newPaymentDate);
        SpendActionResult GetSpendActionResult(int spendId, ResourceActionNames actionType, ApplicationModules applicationModule);
	    IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string userId);
	    IEnumerable<EditSpendViewModel> GetEditSpendViewModel(int accountPeriodId, int spendId, string userId);
	    Task<IEnumerable<SpendItemModified>> AddBasicTransactionAsync(ClientBasicTrxByPeriod clientBasicTrxByPeriod,
		    TransactionTypeIds transactionTypeId);
    }
}