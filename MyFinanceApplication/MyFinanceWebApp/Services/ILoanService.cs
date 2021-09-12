using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System;
using System.Collections.Generic;

namespace MyFinanceWebApp.Services
{
    public interface ILoanService
    {
        IEnumerable<SpendItemModified> AddLoanSpend(ClientLoanSpendViewModel clientLoanSpendViewModel, string token);
        IEnumerable<SpendItemModified> CreateLoan(ClientLoanViewModel clientLoanViewModel, string token);
        AddLoanRecordViewModel GetAddLoanRecordViewModel(int accountId, DateTime dateTime, string token);
        IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByCriteriaId(string token, int loanRecordStatusId, LoanQueryCriteria criteriaId = LoanQueryCriteria.Invalid,
            IEnumerable<int> ids = null);
        LoanReportViewModel GetLoanDetailRecordsById(int loanRecordId, bool lowConsume, string token);
        AddLoanSpendViewModel GetAddLoanSpendViewModel(int loanRecordId, string token);
        IEnumerable<AccountDetailsViewModel> GetSupportedLoanAccount(string token);
        IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountId, DateTime dateTime,
            int currencyId, string token);
        IEnumerable<ItemModified> DeleteLoan(int loanRecordId, string token);
    }
}