using System.Collections.Generic;
using System.Net.Http;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public class WebApiAccountGroupService : MvcWebApiBaseService, IAccountGroupService
	{
		#region Attributes

		protected override string ControllerName => "accountGroup";

		#endregion

		#region Methods

		public void DeleteAccountGroup(string token, int accountGroupId)
		{
			var parameters = new Dictionary<string, object>
			{
				{"accountGroupId", accountGroupId}
			};

			var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Delete);
            GetResponse(request);
		}

	    public int AddAccountGroup(string token, AccountGroupClientViewModel accountGroupViewModel)
	    {
            var url = CreateRootUrl();
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = accountGroupViewModel
            };

	        var result = GetResponseAs<int>(request);
	        return result;
	    }

	    public int EditAccountGroup(string token, AccountGroupClientViewModel accountGroupViewModel)
	    {
            var url = CreateRootUrl();
            var request = new WebApiRequest(url, new HttpMethod("PATCH"), token)
            {
                Model = accountGroupViewModel
            };

            var result = GetResponseAs<int>(request);
            return result;
	    }

	    public AccountGroupDetailViewModel GetAccountGroupDetailViewModel(string token ,int accountGroupId)
		{
			var url = CreateCustomUrl(accountGroupId.ToString());
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var result = GetResponseAs<AccountGroupDetailViewModel>(request);
			return result;
		}

		public IEnumerable<AccountGroupViewModel> GetAccountGroupViewModel(string token)
		{
			var url = CreateRootUrl();
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var results = GetResponseAs<IEnumerable<AccountGroupViewModel>>(request);
			return results;
		}

		public IEnumerable<AccountGroupPosition> GetAccountGroupPositions(string token, bool validateAdd = false,
			int accountGroupIdSelected = 0)
		{
			var parameters = new Dictionary<string, object>
			{
				{"validateAdd", validateAdd},
				{"accountGroupIdSelected", accountGroupIdSelected}
			};

			var url = CreateMethodUrl("positions", parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var results = GetResponseAs<IEnumerable<AccountGroupPosition>>(request);
			return results;
		}

		#endregion

		public WebApiAccountGroupService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
		{
		}
	}
}