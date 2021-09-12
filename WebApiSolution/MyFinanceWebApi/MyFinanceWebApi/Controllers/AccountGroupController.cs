using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyFinanceBackend.Services;
using MyFinanceModel.ViewModel;
using MyFinanceModel.ClientViewModel;

namespace MyFinanceWebApi.Controllers
{
	public class AccountGroupController : BaseController
	{
		#region Attributes

		private readonly IAccountGroupService _accountGroupService;

		private const string ROOT_ROUTE = "api/accountGroup/";

		#endregion

		#region Constructor

		public AccountGroupController(IAccountGroupService accountGroupService)
		{
			_accountGroupService = accountGroupService;
		}

        #endregion

        #region Routes

	    [Route(ROOT_ROUTE)]
	    [HttpDelete]
	    public HttpResponseMessage DeleteAccountGroup(int accountGroupId)
	    {
            var userId = GetUserId();
	        _accountGroupService.DeleteAccountGroup(userId, accountGroupId);
	        return new HttpResponseMessage(HttpStatusCode.OK);
	    }

        [Route(ROOT_ROUTE)]
        [HttpPost]
        public int AddAccountGroup(AccountGroupClientViewModel accountGroupViewModel)
        {
            accountGroupViewModel.AccountGroupId = 0;
            var result = _accountGroupService.AddorEditAccountGroup(accountGroupViewModel);
            if (string.IsNullOrEmpty(accountGroupViewModel.UserId))
            {
                accountGroupViewModel.UserId = GetUserId();
            }

            return result;
        }

        [Route(ROOT_ROUTE)]
        [HttpPatch]
        public int EditccountGroup(AccountGroupClientViewModel accountGroupViewModel)
        {
            var result = _accountGroupService.AddorEditAccountGroup(accountGroupViewModel);
            if (string.IsNullOrEmpty(accountGroupViewModel.UserId))
            {
                accountGroupViewModel.UserId = GetUserId();
            }

            return result;
        }

        [Route(ROOT_ROUTE + "{accountGroupId}")]
	    [HttpGet]
	    public AccountGroupDetailViewModel GetAccountGroupDetailViewModel(int accountGroupId)
	    {
	        var userId = GetUserId();
	        var results = _accountGroupService.GetAccountGroupDetailViewModel(userId, new[] {accountGroupId});
	        var result = results.FirstOrDefault(i => i.AccountGroupId == accountGroupId);
	        return result;
	    }

        [Route(ROOT_ROUTE + "positions")]
        [HttpGet]
        public IEnumerable<AccountGroupPosition> GetAccountGroupPositions(bool validateAdd = false,
            int accountGroupIdSelected = 0)
        {
            var userId = GetUserId();
            var results = _accountGroupService.GetAccountGroupPositions(userId, validateAdd, accountGroupIdSelected);
            return results;
        }

        [Route(ROOT_ROUTE)]
		[HttpGet]
		public IEnumerable<AccountGroupViewModel> GetAccountGroupViewModel()
		{
            var userId = GetUserId();
            var result = _accountGroupService.GetAccountGroupViewModel(userId);
			return result;
		}

		#endregion
	}
}
