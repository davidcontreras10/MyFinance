using MyFinanceBackend.Services;
using MyFinanceModel.ViewModel;
using System.Collections.Generic;
using System.Web.Http;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceWebApi.CustomHandlers;

namespace MyFinanceWebApi.Controllers
{
	[RoutePrefix(ROOT_ROUTE)]
    public class AccountController : BaseController
    {
        #region Attributes

        private readonly IAccountService _accountService;
	    private readonly IAccountFinanceService _accountFinanceService;

		private const string ROOT_ROUTE = "api/account";

        #endregion

        #region Constructor

        public AccountController(IAccountService accountService, IAccountFinanceService accountFinanceService)
        {
            _accountService = accountService;
	        _accountFinanceService = accountFinanceService;
        }

        #endregion

        #region Routes

        [Route]
        [HttpDelete]
        public void DeleteAccount([FromUri] int accountId)
        {
            var userId = GetUserId();
            _accountService.DeleteAccount(userId, accountId);
        }

        [ValidateModelState]
        [Route]
        [HttpPost]
        public void AddAccount([FromBody]ClientAddAccount clientAddAccount)
        {
            var userId = GetUserId();
            _accountService.AddAccount(userId, clientAddAccount);
        }

	    [Route("supportedAccountInclude")]
	    [HttpGet]
	    public IEnumerable<SupportedAccountIncludeViewModel> GetSupportedAccountIncludeViewModel(
		    [FromUri]ClientAddSpendAccountIncludeUpdate[] listUpdates)
	    {
		    var userId = GetUserId();
		    var supportedAccountIncludeViewModelList = _accountService.GetSupportedAccountIncludeViewModel(listUpdates, userId);
		    return supportedAccountIncludeViewModelList;
	    }

		[Route("finance")]
	    [HttpGet]
	    [IncludeRestrictObjectHeader]
	    public IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel([FromUri]ClientAccountFinanceViewModel[] accountPeriods)
	    {
		    var userId = GetUserId();
		    var accountFinanceViewModelList = _accountFinanceService.GetAccountFinanceViewModel(accountPeriods, userId);
		    return accountFinanceViewModelList;
	    }

	    [Route("finance/summary")]
	    [HttpGet]
	    public IEnumerable<BankAccountSummary> GetAccountFinanceSummaryViewModel()
	    {
		    var userId = GetUserId();
		    var accounts = _accountFinanceService.GetAccountFinanceSummaryViewModel(userId);
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

		[Route("{accountGroupId}")]
        [HttpGet]
        public AccountMainViewModel GetAccountDetailsViewModel(int? accountGroupId = null)
        {
            var userId = GetUserId();
            var result = _accountService.GetAccountDetailsViewModel(userId, accountGroupId);
            return result;
        }

        [Route]
        [HttpGet]
        public IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsInfoViewModel([FromUri]int[] accountIds)
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
        public AddAccountViewModel GetAddAccountViewModel()
        {
            var userId = GetUserId();
            var result = _accountService.GetAddAccountViewModel(userId);
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

        [Route]
        [HttpPatch]
        public void UpdateAccount([FromBody]ClientEditAccount clientEditAccount)
        {
            var userId = GetUserId();
            _accountService.UpdateAccount(userId, clientEditAccount);
        }

        #endregion
    }
}