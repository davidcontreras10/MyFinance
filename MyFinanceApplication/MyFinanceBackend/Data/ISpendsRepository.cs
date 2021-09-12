using System;
using System.Collections.Generic;
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
		AccountFinanceViewModel GetAccountFinanceViewModel(int accountPeriodId, string userId);
		IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel(IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId);
		IEnumerable<SpendItemModified> DeleteSpend(string userId, int spendId);
        DateRange GetDateRange(string accountIds, DateTime? dateTime, string userId);
        IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model);
        IEnumerable<AccountCurrencyPair> GetAccountsCurrency(IEnumerable<int> accountIdsArray);
        IEnumerable<SpendItemModified> AddSpend(ClientAddSpendModel clientAddSpendModel);
        IEnumerable<SpendItemModified> AddSpend(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId);
        IEnumerable<SavedSpend> GetSavedSpends(int spendId);
        IEnumerable<SpendItemModified> EditSpend(FinanceSpend financeSpend);
        IEnumerable<ClientAddSpendAccount> GetAccountMethodConversionInfo(int? accountId, int? accountPeriodId,
            string userId, int currencyId);
        ClientAddSpendModel CreateClientAddSpendModel(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId);
        IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string userId);
        void AddSpendDependency(int spendId, int dependencySpendId);
        SpendActionAttributes GetSpendAttributes(int spendId);
    }
}
