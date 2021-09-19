using System;
using System.Collections.Generic;
using MyFinance.MyFinanceModel.ClientViewModel;

namespace MyFinance.MyFinanceModel
{
    public interface ISpendCurrencyConvertible
    {
        ClientAddSpendAccount OriginalAccountData { get; }
        IEnumerable<ClientAddSpendAccount> IncludedAccounts { get; }
        int CurrencyId { get; }
        DateTime PaymentDate { get; }
    }
}
