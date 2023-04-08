using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
    public class WebApiSpendTypeService : MvcWebApiBaseService, ISpendTypeService
    {
        #region Attributes

        protected override string ControllerName => CoreVersion ? "spendTypes" : "spendType";

	    #endregion

        #region Methods

        public IEnumerable<SpendTypeViewModel> GetSpendTypeByAccountViewModels(string token, int? accountId)
        {
            var parameters = accountId != null && accountId > 0
                ? new Dictionary<string, object>
                {
                    {"AccountId", accountId}
                }
                : null;
            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<IEnumerable<SpendTypeViewModel>>(request);
            return response;
        }

        public IEnumerable<SpendTypeViewModel> GetAllSpendTypes(string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"includeAll", true}
            };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<IEnumerable<SpendTypeViewModel>>(request);
            return response;
        }

        public async Task<IEnumerable<SpendTypeViewModel>> GetAllSpendTypesAsync(string token)
        {
	        var parameters = new Dictionary<string, object>
	        {
		        {"includeAll", true}
	        };

	        var url = CreateRootUrl(parameters);
	        var request = new WebApiRequest(url, HttpMethod.Get, token);
	        var response = await GetResponseAsAsync<IEnumerable<SpendTypeViewModel>>(request);
	        return response;
        }

        public IEnumerable<SpendTypeViewModel> GetUserSpendTypes(string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"includeAll", false}
            };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<IEnumerable<SpendTypeViewModel>>(request);
            return response;
        } 

        public IEnumerable<int> AddSpendTypes(string token, ClientSpendType clientSpendType)
        {
            var url = CreateRootUrl();
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = clientSpendType
            };

            var response = GetResponseAs<IEnumerable<int>>(request);
            return response;
        }

        public IEnumerable<int> EditSpendTypes(string token, ClientSpendType clientSpendType)
        {
            var url = CreateRootUrl();
            var request = new WebApiRequest(url, new HttpMethod("PATCH"), token)
            {
                Model = clientSpendType
            };

            var response = GetResponseAs<IEnumerable<int>>(request);
            return response;
        }

	    public void DeleteSpendType(string token, int spendTypeId)
	    {
		    var url = CreateRootUrl();
            var request = new WebApiRequest(url, HttpMethod.Delete, token)
            {
                Model = new ClientSpendTypeId { SpendTypeId = spendTypeId }
            };

            var response = GetResponse(request);
		    if (!response.IsSuccessStatusCode)
		    {
			    throw new HttpResponseException(response);
		    }
	    }

	    public IEnumerable<int> DeleteSpendTypeUser(string token, int spendTypeId)
	    {
		    var url = CreateCustomUrl("user");
            var request = new WebApiRequest(url, HttpMethod.Delete, token)
            {
                Model = new ClientSpendTypeId { SpendTypeId = spendTypeId }
            };

            var response = GetResponseAs<IEnumerable<int>>(request);
		    return response;
	    }

	    public IEnumerable<int> AddSpendTypeUser(string token, int spendTypeId)
	    {
			var url = CreateCustomUrl("user");
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = new ClientSpendTypeId { SpendTypeId = spendTypeId }
            };

            var response = GetResponseAs<IEnumerable<int>>(request);
		    return response;
	    }

        #endregion

        public WebApiSpendTypeService(IHttpClientFactory httpClientFactory) : base(httpClientFactory, true)
        {
        }
    }
}