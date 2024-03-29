﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Services
{
    public interface ITransferService
    {
        IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string token);
        IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId, string token,
            BalanceTypes balanceType);

        Task<IEnumerable<AccountViewModel>> GetPossibleDestinationAccountAsync(
	        int accountPeriodId,
	        int currencyId,
	        string token,
	        BalanceTypes balanceType
        );
        TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId, string token);
        IEnumerable<ItemModified> SubmitTransfer(string token, TransferClientViewModel transferClientViewModel);
    }
}