using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
{
	public interface IAccountFinanceService
	{
		IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel(
			IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId);
        IEnumerable<BankAccountSummary> GetAccountFinanceSummaryViewModel(string userId);
    }
}