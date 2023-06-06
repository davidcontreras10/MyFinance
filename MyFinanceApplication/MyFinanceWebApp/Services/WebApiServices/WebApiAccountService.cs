using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Contants;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public class WebApiAccountService : MvcWebApiBaseService, IAccountService
	{
		protected override string ControllerName => "accounts";

		public async Task<IReadOnlyCollection<AccountDetailsPeriodViewModel>> BasicUserAccountsAsync(string token)
		{
			var url = CreateMethodUrl("list");
			var request = new WebApiRequest(url, HttpMethod.Get, token);
			return await GetResponseAsAsync<IReadOnlyCollection<AccountDetailsPeriodViewModel>>(request);
		}

		public async Task<IEnumerable<BankAccountSummary>> GetBankAccountSummaryAsync(string token)
		{
			var url = CreateMethodUrl("finance/summary");
			var request = new WebApiRequest(url, HttpMethod.Get, token);
			var accounts = await GetResponseAsAsync<IEnumerable<BankAccountSummary>>(request);
			return accounts;
		}

		public async Task<IEnumerable<AccountFinanceViewModel>> GetSimpleAccountFinanceViewModelAsync(IEnumerable<ClientAccountFinanceViewModel> accountPeriods, string token)
		{

			var appServiceHeader = ServiceAppHeader.GetServiceAppHeader(ServiceAppHeader.ServiceAppHeaderType.Restrict);
			var headers = new Dictionary<string, string>
			{
				{appServiceHeader.Name, "[].SpendViewModels" }
			};

			var url = CreateMethodUrl("finance");
			var request = new WebApiRequest(url, HttpMethod.Post, token)
			{
				Headers = headers,
				Model = accountPeriods
			};

			var accountFinanceViewModelList = await GetResponseAsAsync<IEnumerable<AccountFinanceViewModel>>(request);
			return accountFinanceViewModelList;
		}

		public async Task<IEnumerable<AccountFinanceViewModel>> GetAccountFinanceViewModelAsync(IEnumerable<int> accountPeriodIds, bool isPending, string token)
		{
			if (accountPeriodIds == null || !accountPeriodIds.Any())
				return new List<AccountFinanceViewModel>();

			var accountPeriods = accountPeriodIds.Select(accountPeriodId => new ClientAccountFinanceViewModel
			{
				AccountPeriodId = accountPeriodId,
				LoanSpends = false,
				PendingSpends = isPending
			});

			var url = CreateMethodUrl("finance");
			var request = new WebApiRequest(url, HttpMethod.Post, token)
			{
				Model = accountPeriods
			};
			var accountFinanceViewModelList = await GetResponseAsAsync<IEnumerable<AccountFinanceViewModel>>(request);
			return accountFinanceViewModelList;
		}

		public async Task<UserAccountsViewModel> GetAccountsByUserIdAsync(string token)
		{
			var url = CreateMethodUrl("user");
			var request = new WebApiRequest(url, HttpMethod.Get, token);
			var result = await GetResponseAsAsync<UserAccountsViewModel>(request);
			return result;
		}

		public AccountMainViewModel GetAccountDetailsViewModel(string token, int accountGroupId)
		{
			var url = CreateCustomUrl(accountGroupId.ToString());
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var result = GetResponseAs<AccountMainViewModel>(request);
			return result;
		}

		public IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsInfoViewModel(IEnumerable<int> accountIds, string token)
		{
			var parameters = new Dictionary<string, object>
            {
                {"accountIds", accountIds}
            };

			var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var result = GetResponseAs<IEnumerable<AccountDetailsInfoViewModel>>(request);
			return result;
		}

		public IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(string token, int currencyId)
		{
			var method = WebServicesConstants.GET_ACCOUNT_INCLUDE_VIEW_MODEL + "/" + currencyId;
			var url = CreateCustomUrl(method);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var result = GetResponseAs<IEnumerable<AccountIncludeViewModel>>(request);
			return result;
		}

		public IEnumerable<ItemModified> UpdateAccountPositions(string token, IEnumerable<ClientAccountPosition> accountPositions)
	    {
			var url = CreateMethodUrl(WebServicesConstants.UPDATE_ACCOUNT_POSITIONS);
            var request = new WebApiRequest(url, HttpMethod.Put, token)
            {
                Model = accountPositions
            };

            var result = GetResponseAs<IEnumerable<ItemModified>>(request);
			return result;
	    }

		public void UpdateAccount(string token, ClientEditAccount clientEditAccount)
		{
			var url = ControllerName;
            var request = new WebApiRequest(url, new HttpMethod("PATCH"), token)
            {
                Model = clientEditAccount
            };

            GetResponse(request);
		}

		public AddAccountViewModel GetAddAccountViewModel(string token)
		{
			var url = CreateMethodUrl(WebServicesConstants.GET_ADD_ACCOUNT_VIEW_MODEL);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var result = GetResponseAs<AddAccountViewModel>(request);
			return result;
		}

        public void AddAccount(string token, ClientAddAccount clientAddAccount)
        {
			var url = ControllerName;
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = clientAddAccount
            };

            GetResponse(request);
        }

	    public void DeleteAccount(string token, int accountId)
	    {
	        var parameters = new Dictionary<string, object>
	        {
	            {"accountId", accountId}
	        };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Delete, token);
            GetResponse(request);
	    }

		public WebApiAccountService(IHttpClientFactory httpClientFactory) : base(httpClientFactory, coreVersion: true)
		{
		}
	}
}