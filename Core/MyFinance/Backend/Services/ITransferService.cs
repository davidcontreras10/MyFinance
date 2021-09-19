using System.Collections.Generic;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Services
{
    public interface ITransferService
    {
        IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId,
            string userId, BalanceTypes balanceType);
        IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string userId);
        TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId, string userId);
        IEnumerable<ItemModified> SubmitTransfer(TransferClientViewModel transferClientViewModel);
    }
}