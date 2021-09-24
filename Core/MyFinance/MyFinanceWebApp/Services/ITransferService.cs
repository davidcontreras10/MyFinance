using System.Collections.Generic;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.MyFinanceWebApp.Services
{
    public interface ITransferService
    {
        IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string token);
        IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId, string token,
            BalanceTypes balanceType);
        TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId, string token);
        IEnumerable<ItemModified> SubmitTransfer(string token, TransferClientViewModel transferClientViewModel);
    }
}