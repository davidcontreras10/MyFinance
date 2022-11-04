﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyFinanceModel.ClientViewModel;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;

namespace MyFinanceWebApp.Controllers
{
    public class ScheduledTasksController : FinanceAppBaseController
    {
	    private readonly ISpendTypeService _spendTypeService;
        private readonly IHtmlHeaderHelper _htmlHeaderHelper;
        private readonly IAccountService _accountService;
        private readonly ISpendService _spendService;
        private readonly ITransferService _transferService;

        public ScheduledTasksController(
	        IHtmlHeaderHelper htmlHeaderHelper,
	        IAccountService accountService,
	        ISpendTypeService spendTypeService,
	        ISpendService spendService,
	        ITransferService transferService
        )
        {
	        _htmlHeaderHelper = htmlHeaderHelper;
	        _accountService = accountService;
	        _spendTypeService = spendTypeService;
	        _spendService = spendService;
	        _transferService = transferService;
        }


        // GET: ScheduledTasks
        public ActionResult Index()
        {
            var model = TempData.ContainsKey("model")
                ? TempData["model"] as ScheduledTasksViewModel
                : new ScheduledTasksViewModel
                {
                    HeaderModel = CreateMainHeaderModel(),
                    HtmlHeaderHelper = _htmlHeaderHelper,
                    RequestType = ScheduleTaskRequestType.View
                };
            return View(model);
        }

        // GET: ScheduledTasks
        public ActionResult New(int? accountId = null)
        {
            TempData["model"] = new ScheduledTasksViewModel
            {
                HeaderModel = CreateMainHeaderModel(),
                HtmlHeaderHelper = _htmlHeaderHelper,
                RequestType = ScheduleTaskRequestType.New
            };

            return RedirectToAction("Index");
        }

        [JsonErrorHandling]
        [HttpGet]
        public async Task<ActionResult> GetUserAccountsAsync()
        {
	        var authToken = GetUserToken();
            var mainViewModelData = await _accountService.GetAccountsByUserIdAsync(authToken);
            var accounts = new List<UserSelectAccount>();
            foreach (var mv in mainViewModelData.AccountGroupMainViewViewModels)
            {
	            accounts.AddRange(mv.Accounts.Select(account => new UserSelectAccount
	            {
		            AccountName = account.AccountName, AccountId = account.AccountId,
		            AccountPeriodId = account.CurrentPeriodId
	            }));
            }

            return JsonCamelCaseResult(accounts);
        }

        public async Task<ActionResult> GetAddSpendViewModelAsync(int accountPeriodId)
        {
	        var authToken = GetUserToken();
	        var addSpendViewModelList = await _spendService
		        .GetAddSpendViewModelAsync(new[] { accountPeriodId }, authToken);
	        var addSpendViewModel = addSpendViewModelList.FirstOrDefault(item => item.AccountPeriodId == accountPeriodId);
	        if (addSpendViewModel == null)
		        throw new HttpException(404, "Data not found");
	        return JsonCamelCaseResult(addSpendViewModel);
        }

        public async Task<ActionResult> GetPossibleDestinationAccountAsync(int accountPeriodId, int currencyId)
        {
	        var authToken = GetUserToken();
	        var accounts = await _transferService.GetPossibleDestinationAccountAsync(accountPeriodId, currencyId, authToken,
		        BalanceTypes.Custom);
	        return JsonCamelCaseResult(accounts);
        }

        public async Task<ActionResult> GetSpendingTypesAsync()
        {
	        var authToken = GetUserToken();
	        var spendTypes = await _spendTypeService.GetAllSpendTypesAsync(authToken);
	        return JsonCamelCaseResult(spendTypes);

        }

        private MainHeaderModel CreateMainHeaderModel()
        {
            return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.ScheduledTasks);
        }
    }
}