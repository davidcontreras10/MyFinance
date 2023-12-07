using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.App_Utils;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;

namespace MyFinanceWebApp.Controllers
{
	[HandleTokenError]
	public class HomeController : FinanceAppBaseController
	{
		#region Constructor

		public HomeController(ISpendService spendService, ITransferService transferService, IHtmlHeaderHelper htmlHeaderHelper, IAccountService accountService)
		{
			SpendService = spendService ?? throw new ArgumentNullException(nameof(spendService));
			AccountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
			TransferService = transferService;
			HtmlHeaderHelper = htmlHeaderHelper;
		}

		#endregion

		#region Attributes

		private IHtmlHeaderHelper HtmlHeaderHelper { get; }
		private ITransferService TransferService { get; }
		private ISpendService SpendService { get; }
		private IAccountService AccountService { get; }

		#endregion

		#region Submit Actions

		public async Task<ActionResult> Index()
		{
			var authToken = GetUserToken();
			var mainViewModelData = await AccountService.GetAccountsByUserIdAsync(authToken);
			var result = new MainViewPageModel
			{
				Model = mainViewModelData,
				HeaderModel = CreateMainHeaderModel(),
				HtmlHeaderHelper = HtmlHeaderHelper
			};

			return View("MainView", result);
		}

		public ActionResult GetMainViewModelData()
		{
			return RedirectToAction("Index");
		}

		#endregion

		#region Json Response

		[JsonErrorHandling]
		[HttpGet]
		public async Task<ActionResult> GetAccountFileAsync(int accountPeriodId, bool isPending)
		{
			var accountViewModels =
				await AccountService.GetAccountFinanceViewModelAsync(new[] {accountPeriodId}, isPending,
					GetUserToken());
			var bytes = ExcelFileHelper.GenerateFile(accountViewModels.ToList());
			var response = new
			{
				bytes,
				fileName = ExcelFileHelper.GetFileName(accountViewModels)
			};
			return JsonCamelCaseResult(response);
		}

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetAddTrxViewModel(int accountPeriodId)
		{
			var authToken = GetUserToken();
			var addSpendViewModelList = SpendService
				.GetAddSpendViewModel(new[] { accountPeriodId }, authToken);
			var addSpendViewModel = addSpendViewModelList.FirstOrDefault(item => item.AccountPeriodId == accountPeriodId);
			if (addSpendViewModel == null)
				throw new HttpException(404, "Data not found");
			return JsonCamelCaseResult(addSpendViewModel);
		}

		[JsonErrorHandling]
		[HttpPost]
		public async Task<ActionResult> AddBasicSpend(AddBasicTrxByPeriod model)
		{
			var userToken = GetUserToken();
			var requestModel = CreateClientBasicTrxByPeriod(model);
			var accountsModified = await SpendService.AddBasicSpendAsync(userToken, requestModel);
			return JsonCamelCaseResult(accountsModified);
		}

		[JsonErrorHandling]
		[HttpPost]
		public async Task<ActionResult> AddBasicIncome(AddBasicTrxByPeriod model)
		{
			var userToken = GetUserToken();
			var requestModel = CreateClientBasicTrxByPeriod(model);
			var accountsModified = await SpendService.AddBasicIncomeAsync(userToken, requestModel);
			return JsonCamelCaseResult(accountsModified);
		}

		[JsonErrorHandling]
		[HttpGet]
		public async Task<ActionResult> GetAccountsSummary(IEnumerable<int> accountIds = null)
		{
			var userToken = GetUserToken();
			var accounts = await AccountService.GetBankAccountSummaryAsync(userToken);
            if (accountIds != null && accountIds.Any())
            {
                accounts = accounts.Where(acc => accountIds.Any(selectedAcc => selectedAcc == acc.AccountId));
            }

            var viewModel = new BankAccountSummaryViewModel
            {
                AccountIds = accountIds,
                Summary = accounts
            };

            return JsonCamelCaseResult(viewModel);
		}

		[JsonErrorHandling]
		[HttpGet]
		public async Task<ActionResult> GetSimpleAccountFinanceViewModel(int accountPeriodId, bool defaultValues, bool loanSpends,
			bool pendingSpends, int amountTypeId)
		{
			var request = defaultValues
				? new ClientAccountFinanceViewModel
				{
					AccountPeriodId = accountPeriodId
				}
				: new ClientAccountFinanceViewModel
				{
					AccountPeriodId = accountPeriodId,
					AmountTypeId = amountTypeId,
					LoanSpends = loanSpends,
					PendingSpends = pendingSpends
				};

			var userToken = GetUserToken();
			var response = await AccountService.GetSimpleAccountFinanceViewModelAsync(new[] {request}, userToken);
			var mvcResponse = new MvcCustomPeriodAccountData();
			ObjectUtils.CopyFields(request, mvcResponse);
			mvcResponse.AccountFinanceViewModels = response;
			return JsonCamelCaseResult(mvcResponse);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult ConfirmPendingSpend(int spendId)
		{
			if(spendId == 0)
			{
				throw new ArgumentException("SpendId cannot be zero", nameof(spendId));
			}

			var token = GetUserToken();
			var result = SpendService.ConfirmPendingSpend(spendId, token);
			return Json(result);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult EditSpend(ClientEditSpendModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			if (model.SpendId == 0 || !model.ModifyList.Any())
			{
				throw new ArgumentException("Invalid parameters");
			}

			var actionResult = GetEditSpendActionResult(model.SpendId);
			if (actionResult.Result != SpendActionAttributes.ActionResult.Valid)
			{
				throw new Exception("Unable to edit transaction");
			}

			var token = GetUserToken();
			var result = SpendService.EditSpend(model, token);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult SubmitTransfer(AddTransferModel addTransferModel)
		{
			if (addTransferModel == null)
			{
				throw new ArgumentNullException(nameof(addTransferModel));
			}

			if (addTransferModel.BalanceType == BalanceTypes.Invalid)
			{
				throw new Exception("Invalid balance type");
			}

			var authToken = GetUserToken();
			var userId = HttpContext.User.Identity.Name;
			var transferClientViewModel = new TransferClientViewModel
			{
				AccountPeriodId = addTransferModel.AccountPeriodId,
				SpendTypeId = addTransferModel.SpendTypeId,
				Description = addTransferModel.Description,
				DestinationAccount = addTransferModel.DestinationAccountId,
				UserId = userId,
				CurrencyId = addTransferModel.AmountCurrencyId,
				SpendDate = addTransferModel.TransferDateTime.GetDate(),
				BalanceType = addTransferModel.BalanceType,
				Amount = addTransferModel.BalanceType == BalanceTypes.Custom ? addTransferModel.Amount : 1,
				IsPending = addTransferModel.IsPending,
			};

			var result = TransferService.SubmitTransfer(authToken, transferClientViewModel);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetPossibleDestinationAccount(int accountPeriodId, int currencyId, BalanceTypes balanceType)
		{
			var authToken = GetUserToken();
			var result = TransferService.GetPossibleDestinationAccount(accountPeriodId, currencyId, authToken,
				balanceType);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetTranseferAccountFinanceViewModel(int accountPeriodId)
		{
			var authToken = GetUserToken();
			var result = TransferService.GetBasicAccountInfo(accountPeriodId, authToken);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult AddIncome(AddSpendDataModel addSpendDataModel)
		{
			var authToken = GetUserToken();
			var clientAddSpendModel = CreateClientAddSpendModel(addSpendDataModel, authToken);
			var modifiedList = SpendService.AddIncome(authToken, clientAddSpendModel);
			return Json(modifiedList, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult AddSpendCurrency(AddSpendDataModel addSpendDataModel)
		{
			var authToken = GetUserToken();
			var userId = HttpContext.User.Identity.Name;
			var clientAddSpendModel = CreateClientAddSpendModel(addSpendDataModel, userId);
			var modifiedList = SpendService.AddSpendCurrency(authToken, clientAddSpendModel);
			return Json(modifiedList, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpGet]
		public async Task<ActionResult> GetAccountFinanceViewModel(bool isPending, IEnumerable<string> accountPeriodIds)
		{
			var authToken = GetUserToken();
			var accountPeriodIdsArray = GetIdsCollection(accountPeriodIds);
			var accountFinanceViewModelList = await 
				AccountService.GetAccountFinanceViewModelAsync(accountPeriodIdsArray, isPending, authToken);
			InternalUtilities.AddBasicTableProperty(accountFinanceViewModelList);
			return Json(accountFinanceViewModelList, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetEditSpendViewModel(int accountPeriodId, int spendId)
		{
			var authToken = GetUserToken();
			var addSpendViewModelList = SpendService
				.GetEditSpendViewModel(new[] { accountPeriodId }, authToken, spendId);
			//var addSpendViewModel = addSpendViewModelList.FirstOrDefault(item => item.AccountPeriodId == accountPeriodId);
			var addSpendViewModel = addSpendViewModelList.FirstOrDefault();
			if (addSpendViewModel == null)
				throw new HttpException(404, "Data not found");
			return Json(addSpendViewModel, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpGet]
		public ActionResult GetAddSpendViewModel(int accountPeriodId)
		{
			var authToken = GetUserToken();
			var addSpendViewModelList = SpendService
												.GetAddSpendViewModel(new[] { accountPeriodId }, authToken);
			var addSpendViewModel = addSpendViewModelList.FirstOrDefault(item => item.AccountPeriodId == accountPeriodId);
			if (addSpendViewModel == null)
				throw new HttpException(404, "Data not found");
			return Json(addSpendViewModel, JsonRequestBehavior.AllowGet);
		}

		[JsonErrorHandling]
		[HttpPost]
		public ActionResult DeleteSpend(int spendId)
		{
			if (spendId == 0)
			{
				throw new ArgumentException("spendId");
			}

			//var actionResult = GetDeleteSpendActionResult(spendId);
			//if(actionResult.Result != SpendActionAttributes.ActionResult.Valid)
			//{
			//	throw new Exception("Unable to delete transaction");
			//}

            var authToken = GetUserToken();
			var itemsModified = SpendService.DeleteSpend(authToken, spendId);
			return Json(itemsModified, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Private Methods

		private SpendActionResult GetDeleteSpendActionResult(int spendId)
		{
			var token = GetUserToken();
			var response = SpendService.GetSpendActionResult(spendId, MyFinanceModel.ResourceActionNames.Delete, MyFinanceModel.ApplicationModules.MainSpend, token);
			return response;
		}

		private SpendActionResult GetEditSpendActionResult(int spendId)
		{
			var token = GetUserToken();
			var response = SpendService.GetSpendActionResult(spendId, MyFinanceModel.ResourceActionNames.Edit, MyFinanceModel.ApplicationModules.MainSpend, token);
			return response;
		}

		private MainHeaderModel CreateMainHeaderModel()
		{
			return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.Home);
		}

		private static IEnumerable<ClientAddSpendAccountIncludeUpdate> CreateClientAddSpendAccountIncludeUpdates(
			AddSpendUpdateAccountIncludeModel addSpendUpdateAccountIncludeModel)
		{
			if (addSpendUpdateAccountIncludeModel?.IncludedAccounts == null)
				throw new ArgumentNullException(nameof(addSpendUpdateAccountIncludeModel));
			var includeUpdates = addSpendUpdateAccountIncludeModel.IncludedAccounts.Select(
				item =>
					CreateClientAddSpendAccountIncludeUpdate(item, addSpendUpdateAccountIncludeModel.AccountId,
						addSpendUpdateAccountIncludeModel.CurrencyId));
			return includeUpdates;
		}

		private static ClientAddSpendAccountIncludeUpdate CreateClientAddSpendAccountIncludeUpdate(
			AddSpendAccountDataModel addSpendAccountDataModel, int accountId, int currencyId)
		{
			if (addSpendAccountDataModel == null)
				throw new ArgumentNullException(nameof(addSpendAccountDataModel));
			if (accountId == 0 || currencyId == 0)
				throw new ArgumentException("Account or currency cannot be zero");
			return new ClientAddSpendAccountIncludeUpdate
			{
				AccountId = accountId,
				AmountCurrencyId = currencyId,
				AccountIncludeId = addSpendAccountDataModel.AccountId,
				CurrencyConverterMethodId = addSpendAccountDataModel.ConvertionMethodId
			};
		}

		private static ClientBasicTrxByPeriod CreateClientBasicTrxByPeriod(AddBasicTrxByPeriod addBasicTrxByPeriod)
		{
			if (addBasicTrxByPeriod == null)
			{
				return null;
			}

			return new ClientBasicTrxByPeriod
			{
				AccountPeriodId = addBasicTrxByPeriod.AccountPeriodId,
				Amount = addBasicTrxByPeriod.Amount,
				AmountDenominator = 1,
				AmountNumerator = 1,
				SpendTypeId = addBasicTrxByPeriod.SpendTypeId,
				CurrencyId = addBasicTrxByPeriod.CurrencyId,
				Description = addBasicTrxByPeriod.Description,
				IsPending = addBasicTrxByPeriod.IsPending,
				SpendDate = addBasicTrxByPeriod.SpendDate.GetDate(),
			};
		}

		private static ClientAddSpendModel CreateClientAddSpendModel(AddSpendDataModel addSpendDataModel,
																	 string userId)
		{
			if (addSpendDataModel == null)
				return null;
			addSpendDataModel.IncludedAccounts = addSpendDataModel.IncludedAccounts ??
												 new List<AddSpendAccountDataModel>();
			return new ClientAddSpendModel
			{
				Amount = addSpendDataModel.Amount,
				CurrencyId = addSpendDataModel.CurrencyId,
				SpendDate = addSpendDataModel.SpendDate.GetDate(),
				SpendTypeId = addSpendDataModel.SpendTypeId,
				UserId = userId,
				OriginalAccountData = CreateClientAddSpendAccount(addSpendDataModel.OriginalAccountData),
				IncludedAccounts = addSpendDataModel.IncludedAccounts.Select(CreateClientAddSpendAccount),
				Description = addSpendDataModel.Description,
				IsPending = addSpendDataModel.IsPending
			};
		}

		private static ClientAddSpendAccount CreateClientAddSpendAccount(
			AddSpendAccountDataModel addSpendAccountDataModel)
		{
			if (addSpendAccountDataModel == null)
				throw new ArgumentNullException(nameof(addSpendAccountDataModel));
			return new ClientAddSpendAccount
				{
					AccountId = addSpendAccountDataModel.AccountId,
					ConvertionMethodId = addSpendAccountDataModel.ConvertionMethodId
				};
		}

		#endregion

		#region Commented

		//private static string GetSpendModifiedFields(int spendId, string dateString, float amount,
		//                       string accountIds, string description, int spendTypeId, string originalSpend)
		//{
		//    BasicSpend basic = BasicSpend.CreateBasicSpend(originalSpend);
		//    if (basic.Id != spendId)
		//    {
		//        throw new Exception("Spends are different");
		//    }

		//    Spend.Fields fields = new Spend().SpendFields;
		//    string result = "";

		//    //compares date
		//    CustomDate originalDate = basic.GetCustomDate();
		//    CustomDate clientDate = CustomDate.CreateCustomDate(dateString);
		//    if (clientDate != null && !clientDate.Equals(originalDate, false))
		//    {
		//        result += fields.Date + ",";
		//    }

		//    //compare amount
		//    if (basic.Amount != amount)
		//    {
		//        result += fields.Amount + ",";
		//    }

		//    //account ids
		//    if (!SpendAccountIdsEqual(basic.AccountIds, accountIds))
		//    {
		//        result += fields.AccountIds + ",";
		//    }

		//    //description
		//    if (basic.Description != description)
		//    {
		//        result += fields.Description + ",";
		//    }
		//    if (spendTypeId != 0 && basic.SpendTypeId != spendTypeId)
		//    {
		//        result += fields.SpendTypeId + ",";
		//    }
		//    result = SystemDataUtilities.TrimStringList(result, ',');
		//    return result;
		//}

		//private static bool SpendAccountIdsEqual(string original, string client)
		//{
		//    if (string.IsNullOrEmpty(client))
		//        return true;
		//    string[] originalArray = original.Split(',');
		//    string[] clientArray = client.Split(',');
		//    if (clientArray.Length != originalArray.Length)
		//        return false;
		//    return !clientArray.Any(cs => originalArray.All(os => os != cs));
		//}

		#endregion

		#region Test

		[TokenAuthorize]
		public ActionResult TestAuthRoute()
		{
			return new EmptyResult();
		}

		#endregion
	}
}