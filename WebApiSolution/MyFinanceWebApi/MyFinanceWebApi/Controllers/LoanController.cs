using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApi.CustomHandlers;

namespace MyFinanceWebApi.Controllers
{
    [RoutePrefix(ROOT_ROUTE)]
    public class LoanController : BaseController
	{
        #region Attributes

        private const string ROOT_ROUTE = "api/loan";
        private readonly ILoanService _loanService;

        #endregion

        #region Constructor

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        #endregion

        #region Api

        [HttpDelete]
        [Route]
        public IEnumerable<ItemModified> DeleteLoan(int loanRecordId)
        {
            var userId = GetUserId();
            var result = _loanService.DeleteLoan(loanRecordId, userId);
            return result;
        }

        [HttpPost]
        [Route]
        public IEnumerable<SpendItemModified> CreateLoan(ClientLoanViewModel clientLoanViewModel)
        {
            if (clientLoanViewModel == null)
            {
                throw new ArgumentNullException(nameof(clientLoanViewModel));
            }

            clientLoanViewModel.UserId = GetUserId();
            var response = _loanService.CreateLoan(clientLoanViewModel);
            return response;
        }

        [Route("payment")]
        [HttpPost]
        public IEnumerable<SpendItemModified> AddPayment(ClientLoanSpendViewModel clientLoanSpendViewModel)
        {
            if (clientLoanSpendViewModel == null)
            {
                throw new ArgumentNullException(nameof(clientLoanSpendViewModel));
            }

            clientLoanSpendViewModel.UserId = GetUserId();
            var response = _loanService.AddLoanSpend(clientLoanSpendViewModel);
            return response;
        }

        [Route("accounts")]
        [HttpGet]
        public IEnumerable<AccountDetailsViewModel> GetSupportedLoanAccount()
        {
            var userId = GetUserId();
            var result = _loanService.GetSupportedLoanAccount(userId);
            return result;
        }

        [Route("add")]
        [HttpGet]
        public AddLoanRecordViewModel GetAddLoanRecordViewModel(int accountId, DateTime dateTime)
        {
            if (accountId == 0)
            {
                throw new ArgumentException(nameof(accountId));
            }

            var userId = GetUserId();
            var response = _loanService.GetAddLoanRecordViewModel(dateTime, accountId, userId);
            return response;
        }

        [Route("add/payment")]
        [HttpGet]
        public AddLoanSpendViewModel GetAddLoanSpendViewModel(int loanRecordId)
        {
            if (loanRecordId == 0)
            {
                throw new ArgumentException(nameof(loanRecordId));
            }

            var userId = GetUserId();
            var response = _loanService.GetAddLoanSpendViewModel(loanRecordId, userId);
            return response;
        }

        [Route("{loanRecordId}")]
        [HttpGet]
        [IncludeRestrictObjectHeader]
        public LoanReportViewModel GetLoanReportViewModelByLoanRecordId(int loanRecordId)
        {
            var response = _loanService.GetLoanDetailRecordsByIds(new[] {loanRecordId});
            return response.FirstOrDefault();
        }

        [HttpGet]
        [Route]
        public IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByCriteriaId([FromUri]int loanRecordStatusId, [FromUri]LoanQueryCriteria criteriaId, [FromUri]int[] ids = null)
        {
            var userId = GetUserId();
            var result = _loanService.GetLoanDetailRecordsByCriteriaId(userId, loanRecordStatusId, criteriaId, ids, ids);
            return result;
        }

        [HttpGet]
        [Route("destinationAccounts")]
        public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountId, DateTime dateTime,
            int currencyId)
        {
            var userId = GetUserId();
            var result = _loanService.GetPossibleDestinationAccount(accountId, dateTime, userId, currencyId);
            return result;
        }

        #endregion
    }
}