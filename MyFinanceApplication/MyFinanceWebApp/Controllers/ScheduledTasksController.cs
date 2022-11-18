using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;
using MyFinanceWebApp.Services.WebApiServices;

namespace MyFinanceWebApp.Controllers
{
    public class ScheduledTasksController : FinanceAppBaseController
    {
	    private readonly ISpendTypeService _spendTypeService;
        private readonly IHtmlHeaderHelper _htmlHeaderHelper;
        private readonly IAccountService _accountService;
        private readonly ISpendService _spendService;
        private readonly ITransferService _transferService;
        private readonly IScheduledTasksService _scheduledTasksService;
        private readonly IExecutedTasksService _executedTasksService;

        public ScheduledTasksController(
	        IHtmlHeaderHelper htmlHeaderHelper,
	        IAccountService accountService,
	        ISpendTypeService spendTypeService,
	        ISpendService spendService,
	        ITransferService transferService,
	        IScheduledTasksService scheduledTasksService,
	        IExecutedTasksService executedTasksService
        )
        {
	        _htmlHeaderHelper = htmlHeaderHelper;
	        _accountService = accountService;
	        _spendTypeService = spendTypeService;
	        _spendService = spendService;
	        _transferService = transferService;
	        _scheduledTasksService = scheduledTasksService;
	        _executedTasksService = executedTasksService;
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
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetUserAccountsAsync()
        {
	        var authToken = GetUserToken();
            var basicAccounts = await _accountService.BasicUserAccountsAsync(authToken);
            var accounts = basicAccounts
	            .OrderBy(x=>x.AccountGroupId)
	            .ThenBy(x=>x.AccountPosition)
	            .Select(ba => new UserSelectAccount
            {
	            AccountId = ba.AccountId,
	            AccountName = ba.AccountName,
	            AccountPeriodId = ba.AccountPeriodId
            });

            return JsonCamelCaseResult(accounts.ToList().AsReadOnly());
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpGet]
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

        [JsonErrorHandling]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetPossibleDestinationAccountAsync(int accountPeriodId, int currencyId)
        {
	        var authToken = GetUserToken();
	        var accounts = await _transferService.GetPossibleDestinationAccountAsync(accountPeriodId, currencyId, authToken,
		        BalanceTypes.Custom);
	        return JsonCamelCaseResult(accounts);
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetSpendingTypesAsync()
        {
	        var authToken = GetUserToken();
	        var spendTypes = await _spendTypeService.GetAllSpendTypesAsync(authToken);
	        return JsonCamelCaseResult(spendTypes);

        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> CreateBasicAsync(ClientScheduledTask.Basic model)
        {
	        var authToken = GetUserToken();
	        await _scheduledTasksService.CreateBasicAsync(model, authToken);
	        return new EmptyResult();
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> CreateTransferAsync(ClientScheduledTask.Transfer model)
        {
	        var authToken = GetUserToken();
	        await _scheduledTasksService.CreateTransferAsync(model, authToken);
	        return new EmptyResult();
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetScheduledTasksAsync()
        {
	        var authToken = GetUserToken();
	        var tasks = await _scheduledTasksService.GetScheduledTasksAsync(authToken);
	        return JsonCamelCaseResult(tasks);
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetExecutedTaskAsync(string taskId)
        {
	        var authToken = GetUserToken();
	        var executedTasks = await _executedTasksService.GetExecutedTaskAsync(taskId, authToken);
	        return JsonCamelCaseResult(executedTasks);
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpDelete]
        public async Task<ActionResult> DeleteScheduledTaskAsync(string taskId)
        {
	        var authToken = GetUserToken();
	        await _scheduledTasksService.DeleteTaskAsync(taskId, authToken);
	        return new EmptyResult();
        }

        [JsonErrorHandling]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> ExecuteTaskAsync([FromBody] ExecuteTaskRequest request)
        {
	        var authToken = GetUserToken();
	        var taskResult = await _scheduledTasksService.ExecuteTaskAsync(authToken, request.TaskId, request.DateTime);
	        return JsonCamelCaseResult(taskResult);
        }

        private MainHeaderModel CreateMainHeaderModel()
        {
            return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.ScheduledTasks);
        }
    }
}