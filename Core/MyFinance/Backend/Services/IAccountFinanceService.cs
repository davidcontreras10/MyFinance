using System.Collections.Generic;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Services
{
	public interface IAccountFinanceService
	{
		IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel(
			IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId);
        IEnumerable<BankAccountSummary> GetAccountFinanceSummaryViewModel(string userId);
    }
}