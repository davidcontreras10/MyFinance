using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceBackend.Models;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Data
{
    public interface ISpendsRepository : ITransactional
    {
	    IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string userId);
	    IEnumerable<EditSpendViewModel> GetEditSpendViewModel(int accountPeriodId, int spendId, string userId);
		Task<AccountFinanceViewModel> GetAccountFinanceViewModelAsync(int accountPeriodId, string userId);
		Task<IEnumerable<AccountFinanceViewModel>> GetAccountFinanceViewModelAsync(IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId);
		Task<IEnumerable<SpendItemModified>> DeleteSpendAsync(string userId, int spendId);
        DateRange GetDateRange(string accountIds, DateTime? dateTime, string userId);
        Task<IEnumerable<SpendItemModified>> EditSpendAsync(ClientEditSpendModel model);
        Task<IEnumerable<AccountCurrencyPair>> GetAccountsCurrencyAsync(IEnumerable<int> accountIdsArray);
        Task<IEnumerable<SpendItemModified>> AddSpendAsync(ClientAddSpendModel clientAddSpendModel);
        Task<IEnumerable<SpendItemModified>> AddSpendAsync(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId);
		IEnumerable<SavedSpend> GetSavedSpends(int spendId);
        Task<IEnumerable<SpendItemModified>> EditSpendAsync(FinanceSpend financeSpend);
        Task<IEnumerable<ClientAddSpendAccount>> GetAccountMethodConversionInfoAsync(int? accountId, int? accountPeriodId,
            string userId, int currencyId);
        Task<ClientAddSpendModel> CreateClientAddSpendModelAsync(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId);
        IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string userId);
        Task AddSpendDependencyAsync(int spendId, int dependencySpendId);
        SpendActionAttributes GetSpendAttributes(int spendId);
    }
}
