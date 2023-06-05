using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
{
	public interface IAccountFinanceService
	{
		IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel(
			IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId);
        Task<IEnumerable<BankAccountSummary>> GetAccountFinanceSummaryViewModelAsync(string userId, DateTime? dateTime = null);
    }
}