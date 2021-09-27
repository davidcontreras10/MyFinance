using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFinance.MyFinanceModel.ViewModel;
using MyFinance.MyFinanceWebApp.Helpers;
using MyFinance.MyFinanceWebApp.Models;
using MyFinance.MyFinanceWebApp.Services;

namespace MyFinance.MyFinanceWebApp.Controllers
{
	[Authorize]
	public class HomeController : AppBaseController
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IHtmlHeaderHelper _htmlHeaderHelper;
		private readonly ITransferService _transferService;
		private readonly ISpendService _spendService;
		private readonly IAccountService _accountService;

		public HomeController(
			ILogger<HomeController> logger,
			IHtmlHeaderHelper htmlHeaderHelper,
			ITransferService transferService,
			ISpendService spendService,
			IAccountService accountService
		)
		{
			_logger = logger;
			_htmlHeaderHelper = htmlHeaderHelper;
			_transferService = transferService;
			_spendService = spendService;
			_accountService = accountService;
		}

		public IActionResult Index()
		{
			var authToken = GetUserToken();
			//var mainViewModelData = await _accountService.GetAccountsByUserIdAsync(authToken);
			var result = new MainViewPageModel
			{
				Model = new UserAccountsViewModel(),
				HeaderModel = CreateMainHeaderModel(),
				HtmlHeaderHelper = _htmlHeaderHelper
			};

			return View("MainView", result);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		private MainHeaderModel CreateMainHeaderModel()
		{
			return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.Home);
		}
	}
}
