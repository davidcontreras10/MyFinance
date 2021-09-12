using System.Collections.Generic;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
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