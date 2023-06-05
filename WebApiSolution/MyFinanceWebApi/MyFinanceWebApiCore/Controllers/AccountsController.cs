using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : BaseApiController
	{
		#region Attributes

		private readonly IAccountService _accountService;
		private readonly IAccountFinanceService _accountFinanceService;

		#endregion

		#region Constructor

		public AccountsController(IAccountService accountService, IAccountFinanceService accountFinanceService)
		{
			_accountService = accountService;
			_accountFinanceService = accountFinanceService;
		}

		#endregion

		#region Routes

		[HttpDelete]
		public void DeleteAccount([FromQuery] int accountId)
		{
			var userId = GetUserId();
			_accountService.DeleteAccount(userId, accountId);
		}

		//[ValidateModelState]
		[HttpPost]
		public void AddAccount([FromBody] ClientAddAccount clientAddAccount)
		{
			var userId = GetUserId();
			_accountService.AddAccount(userId, clientAddAccount);
		}

		[Route("supportedAccountInclude")]
		[HttpGet]
		public IEnumerable<SupportedAccountIncludeViewModel> GetSupportedAccountIncludeViewModel(
			[FromQuery] ClientAddSpendAccountIncludeUpdate[] listUpdates)
		{
			var userId = GetUserId();
			var supportedAccountIncludeViewModelList = _accountService.GetSupportedAccountIncludeViewModel(listUpdates, userId);
			return supportedAccountIncludeViewModelList;
		}

		[Route("finance")]
		[HttpPost]
		//[IncludeRestrictObjectHeader]
		public IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel([FromBody] ClientAccountFinanceViewModel[] accountPeriods)
		{
			var userId = GetUserId();
			var accountFinanceViewModelList = _accountFinanceService.GetAccountFinanceViewModel(accountPeriods, userId);
			return accountFinanceViewModelList;
		}

		[Route("finance/summary")]
		[HttpGet]
		public async Task<IEnumerable<BankAccountSummary>> GetAccountFinanceSummaryViewModel()
		{
			var userId = GetUserId();
			var accounts = await _accountFinanceService.GetAccountFinanceSummaryViewModelAsync(userId);
			return accounts;
		}

		[HttpGet]
		[Route("user")]
		public UserAccountsViewModel GetAccountsByUserId()
		{
			var userId = GetUserId();
			var accounts = _accountService.GetAccountsByUserId(userId);
			return accounts;
		}

		[Route("list")]
		[HttpGet]
		public async Task<IReadOnlyCollection<AccountDetailsPeriodViewModel>> GetAccountDetailsViewModel()
		{
			var userId = GetUserId();
			return await _accountService.GetAccountDetailsPeriodViewModelAsync(userId, DateTime.UtcNow);
		}

		[Route("{accountGroupId}")]
		[HttpGet]
		public AccountMainViewModel GetAccountDetailsViewModel(int? accountGroupId = null)
		{
			var userId = GetUserId();
			var result = _accountService.GetAccountDetailsViewModel(userId, accountGroupId);
			return result;
		}

		[HttpGet]
		public IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsInfoViewModel([FromQuery] int[] accountIds)
		{
			var userId = GetUserId();
			var result = _accountService.GetAccountDetailsViewModel(accountIds, userId);
			return result;
		}

		[Route("include/{currencyId}")]
		[HttpGet]
		public IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(int currencyId)
		{
			var userId = GetUserId();
			var result = _accountService.GetAccountIncludeViewModel(userId, currencyId);
			return result;
		}

		[Route("add")]
		[HttpGet]
		public async Task<AddAccountViewModel> GetAddAccountViewModel()
		{
			var userId = GetUserId();
			var result = await _accountService.GetAddAccountViewModelAsync(userId);
			return result;
		}

		[Route("positions")]
		[HttpPut]
		public IEnumerable<ItemModified> UpdateAccountPositions([FromBody] IEnumerable<ClientAccountPosition> accountPositions)
		{
			var userId = GetUserId();
			var result = _accountService.UpdateAccountPositions(userId, accountPositions);
			return result;
		}

		[HttpPatch]
		public void UpdateAccount([FromBody] ClientEditAccount clientEditAccount)
		{
			var userId = GetUserId();
			_accountService.UpdateAccount(userId, clientEditAccount);
		}

		#endregion
	}
}
