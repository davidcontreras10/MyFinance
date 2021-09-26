using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
