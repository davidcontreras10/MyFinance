using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyFinanceWebApp.Controllers
{
	public class LoanController : FinanceAppBaseController
	{
        #region Attributes

        private readonly ILoanService _loanService;
        private readonly IHtmlHeaderHelper _htmlHeaderHelper;
	    private readonly ISpendService _spendService;

        #endregion

        #region Constructor

        public LoanController(ILoanService loanService, IHtmlHeaderHelper htmlHeaderHelper, ISpendService spendService)
        {
            _loanService = loanService;
            _htmlHeaderHelper = htmlHeaderHelper;
            _spendService = spendService;
        }

        #endregion

        #region Actions

        [HttpGet]
        public ActionResult Index()
		{
            var token = GetUserToken();
            var response = _loanService.GetLoanDetailRecordsByCriteriaId(token, 1);
            var model = CreateLoanPageModel(response);
            model.HtmlHeaderHelper = _htmlHeaderHelper;
            model.HeaderModel = CreateMainHeaderModel(Helpers.PageHeaderBuilder.AppMenuItem.Loan);
            return View(model);
		}

        [HttpPost]
        public ActionResult Index(LoanPageModel model)
        {
            return Index();
        }

		#endregion

		#region Json

		#region Get

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetDeleteSpendActionResult(int spendId)
		{
			var token = GetUserToken();
			var response = _spendService.GetSpendActionResult(spendId, MyFinanceModel.ResourceActionNames.Delete, MyFinanceModel.ApplicationModules.MainSpend, token);
			return JsonCamelCaseResult(response);
		}

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetEditSpendActionResult(int spendId)
		{
			var token = GetUserToken();
			var response = _spendService.GetSpendActionResult(spendId, MyFinanceModel.ResourceActionNames.Edit, MyFinanceModel.ApplicationModules.MainSpend, token);
			return JsonCamelCaseResult(response);
		}

		[JsonErrorHandling]
        [HttpGet]
        public ActionResult GetLoanDetailRecordsById(int loanRecordId)
        {
            var token = GetUserToken();
            var response = _loanService.GetLoanDetailRecordsById(loanRecordId, true, token);
            return JsonCamelCaseResult(response);
        }

        [JsonErrorHandling]
        [HttpGet]
	    public ActionResult GetPossibleDestinationAccount(int accountId, DateTime dateTime, int currencyId)
	    {
	        var token = GetUserToken();
	        var response = _loanService.GetPossibleDestinationAccount(accountId, dateTime, currencyId, token);
	        return JsonCamelCaseResult(response);
	    }

        [JsonErrorHandling]
        [HttpGet]
        public ActionResult GetAddLoanData(int accountId, DateTime dateTime)
        {
            var token = GetUserToken();
            var response = _loanService.GetAddLoanRecordViewModel(accountId, dateTime, token);
            return JsonCamelCaseResult(response);
        }

        [JsonErrorHandling]
        [HttpGet]
	    public ActionResult GetAddPaymentData(int loanRecordId)
	    {
	        var token = GetUserToken();
	        var response = _loanService.GetAddLoanSpendViewModel(loanRecordId, token);
	        return JsonCamelCaseResult(response);
	    }

        [JsonErrorHandling]
        [HttpGet]
	    public ActionResult GetPossibleAccounts()
	    {
	        var token = GetUserToken();
	        var result = _loanService.GetSupportedLoanAccount(token);
	        return JsonCamelCaseResult(result);
	    }

        #endregion

        #region Post

	    [JsonErrorHandling]
	    [HttpPost]
	    public ActionResult DeleteSpend(int spendId)
	    {
	        if (spendId == 0)
	        {
	            throw new ArgumentException("spendId");
	        }

	        var authToken = GetUserToken();
	        var itemModifieds = _spendService.DeleteSpend(authToken, spendId);
	        return JsonCamelCaseResult(itemModifieds);
	    }

        [JsonErrorHandling]
        [HttpPost]
        public ActionResult DeleteLoan(int loanRecordId)
        {
            var token = GetUserToken();
            var response = _loanService.DeleteLoan(loanRecordId, token);
            return JsonCamelCaseResult(response);
        }

        [JsonErrorHandling]
        [HttpPost]
        public ActionResult SubmitPayment(AddLoanPaymentModel addLoanPaymentModel)
        {
            if (addLoanPaymentModel == null)
            {
                throw new ArgumentNullException(nameof(addLoanPaymentModel));
            }

            var token = GetUserToken();
			var submitModel = new ClientLoanSpendViewModel
			{
				Amount = addLoanPaymentModel.FullPayment ? 1 : addLoanPaymentModel.Amount,
				CurrencyId = addLoanPaymentModel.CurrencyId,
				//IsPending = addLoanPaymentModel.IsPending,
                IsPending = false,
				SpendDate = addLoanPaymentModel.DateTime,
				Description = addLoanPaymentModel.Description,
				LoanRecordId = addLoanPaymentModel.LoanRecordId,
				SpendTypeId = 1,
				FullPayment = addLoanPaymentModel.FullPayment
			};

            var response = _loanService.AddLoanSpend(submitModel, token);
            return JsonCamelCaseResult(response);
        }

        [JsonErrorHandling]
        [HttpPost]
        public ActionResult SubmitLoan(AddLoanModel addLoanModel)
        {
            if(addLoanModel == null)
            {
                throw new ArgumentNullException(nameof(addLoanModel));
            }

            if (addLoanModel.SameAsSource)
            {
                addLoanModel.DestinationAccountId = addLoanModel.AccountId;
            }

            addLoanModel.SpendTypeId = 1;
            var token = GetUserToken();
            var response = _loanService.CreateLoan(addLoanModel, token);
            return JsonCamelCaseResult(response);
        }

        #endregion

        #endregion

        #region Data Transform

        private static LoanPageModel CreateLoanPageModel(IEnumerable<LoanReportViewModel> loanReportViewModels)
		{
			if(loanReportViewModels == null || !loanReportViewModels.Any())
			{
				return new LoanPageModel();
			}

			var pageModel = new LoanPageModel
			{
				Acccounts = new List<BasicLoanAccountModel>()
			};

			foreach(var loan in loanReportViewModels)
			{
                var account = pageModel.Acccounts.FirstOrDefault(acc => acc.AccountId == loan.AccountId);
                if(account == null)
                {
                    account = CreateBasicLoanAcccountModel(loan);
                    ((List<BasicLoanAccountModel>)pageModel.Acccounts).Add(account);
                }

                ((List<LoanReportViewModel>)account.Loans).Add(loan);
			}

            return pageModel;
		}

		private static BasicLoanAccountModel CreateBasicLoanAcccountModel(LoanReportViewModel loanReportViewModel)
		{
			if(loanReportViewModel == null)
			{
				throw new ArgumentNullException(nameof(loanReportViewModel));
			}

			var result = new BasicLoanAccountModel
			{
				AccountId = loanReportViewModel.AccountId,
				AccountName = loanReportViewModel.AccountName,
				Loans = new List<LoanReportViewModel>()
			};

			return result;
		}

        #endregion
    }
}