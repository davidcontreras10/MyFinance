using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinanceModel.ClientViewModel;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;
using WebGrease.Css.Extensions;
using System;
using MyFinanceWebApp.Helpers;

namespace MyFinanceWebApp.Controllers
{
	[HandleTokenError]
    public class AccountController : FinanceAppBaseController
	{	
		#region Attributes

		private readonly IAccountService _accountService;
	    private readonly IHtmlHeaderHelper _htmlHeaderHelper;
	    private readonly IAccountGroupService _accountGroupService;

		#endregion

		#region Constructor

	    public AccountController(IAccountService accountService, IHtmlHeaderHelper htmlHeaderHelper,
		    IAccountGroupService accountGroupService)
	    {
		    _accountService = accountService;
		    _htmlHeaderHelper = htmlHeaderHelper;
		    _accountGroupService = accountGroupService;
	    }

	    #endregion

		#region Submit Actions

	    public ActionResult Index(int accountGroupId = -1)
	    {
			var authToken = GetUserToken();
            var accountDetailsViewModel = _accountService.GetAccountDetailsViewModel(authToken, accountGroupId);
		    var model = new AccountDetailsViewPageModel
		    {
			    Model = accountDetailsViewModel,
			    HtmlHeaderHelper = _htmlHeaderHelper,
			    HeaderModel = CreateMainHeaderModel()
		    };

		    return View("Index", model);
	    }

	    #endregion

		#region Json Response

		[JsonErrorHandling]
	    [HttpPost]
	    public ActionResult DeleteAccountGroup(int accountGroupId)
	    {
		    if (accountGroupId == 0)
		    {
			    throw new ArgumentException("Value cannot be zero", "accountGroupId");
		    }

			var authToken = GetUserToken();
            _accountGroupService.DeleteAccountGroup(authToken, accountGroupId);
		    return Json("Success", JsonRequestBehavior.AllowGet);
	    }

		[JsonErrorHandling]
        [HttpPost]
        public ActionResult EditAccountGroup(AccountGroupClientViewModel accountGroupClientViewModel)
        {
            var authToken = GetUserToken();
			var userId = HttpContext.User.Identity.Name;
			accountGroupClientViewModel.UserId = userId;
            var result = _accountGroupService.EditAccountGroup(authToken, accountGroupClientViewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

		[JsonErrorHandling]
        [HttpPost]
        public ActionResult AddAccountGroup(AccountGroupClientViewModel accountGroupClientViewModel)
        {
            var authToken = GetUserToken();
		    var userId = HttpContext.User.Identity.Name;
            accountGroupClientViewModel.UserId = userId;
            var result = _accountGroupService.AddAccountGroup(authToken, accountGroupClientViewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

		[JsonErrorHandling]
        [HttpGet]
        public ActionResult GetAccountGroupDetailViewModel(int accountGroupId = 0)
        {
            var authToken = GetUserToken();
            var model = accountGroupId == 0
                ? null
                : _accountGroupService.GetAccountGroupDetailViewModel(authToken, accountGroupId);
            var positions = _accountGroupService.GetAccountGroupPositions(authToken, accountGroupId == 0, accountGroupId);
            var viewModel = new AccountGroupDetailViewModel
            {
                AccountGroupPositions = positions,
                Model = model
            };

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

		[JsonErrorHandling]
        [HttpGet]
	    public ActionResult GetAccountGroupList(int accountGroupId = 0)
	    {
			var authToken = GetUserToken();
            var list = _accountGroupService.GetAccountGroupViewModel(authToken).OrderBy(i => i.AccountGroupPosition);
	        list.ForEach(item => item.IsSelected = (accountGroupId != 0 && item.AccountGroupId == accountGroupId));
		    
		    var model = new AccountGroupListViewModel
		    {
			    AccountGroupViewModels = list
		    };

		    return Json(model, JsonRequestBehavior.AllowGet);
	    }

		[JsonErrorHandling]
        [HttpPost]
        public ActionResult DeleteAccount(int accountId)
        {
            var authToken = GetUserToken();
            _accountService.DeleteAccount(authToken, accountId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [JsonErrorHandling]
        [HttpPost]
		public ActionResult AddAccount(MvcClientAddAccount clientAddAccount)
        {
            var authToken = GetUserToken();
            _accountService.AddAccount(authToken, clientAddAccount.GetValue());
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAddAccountViewModel()
        {
            var authToken = GetUserToken();
            var result = _accountService.GetAddAccountViewModel(authToken);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult UpdateAccount(MvcClientEditAccount clientEditAccount)
		{
			var authToken = GetUserToken();
            _accountService.UpdateAccount(authToken, clientEditAccount.GetValue());
		    return Json(0, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult UpdateAccountPositions(IEnumerable<ClientAccountPosition> accountPositions)
		{
			if (accountPositions == null || !accountPositions.Any())
			{
				return Index();
			}

			var authToken = GetUserToken();
            var result = _accountService.UpdateAccountPositions(authToken, accountPositions);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpGet]
	    public ActionResult GetAccountDetailsInfoViewModel(int accountId)
	    {
			var authToken = GetUserToken();
            var resultList = _accountService.GetAccountDetailsInfoViewModel(new[] { accountId }, authToken);
		    var result = resultList.First(i => i.AccountId == accountId);
		    return Json(result, JsonRequestBehavior.AllowGet);
	    }

		[JsonErrorHandling]
        [HttpGet]
        public ActionResult GetAccountIncludes(int currencyId)
        {
            var authToken = GetUserToken();
            var results = _accountService.GetAccountIncludeViewModel(authToken, currencyId);
            return Json(results, JsonRequestBehavior.AllowGet);
        } 

		#endregion

		#region Privates

		private MainHeaderModel CreateMainHeaderModel()
		{
		    return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.Account);
		}

		#endregion
	}
}