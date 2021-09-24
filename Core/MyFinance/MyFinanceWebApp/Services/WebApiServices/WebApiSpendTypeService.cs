using System.Collections.Generic;
using System.Net.Http;
using MyFinance.Backend.ServicesExceptions;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;
using MyFinance.WebApiBaseConsumer;

namespace MyFinance.MyFinanceWebApp.Services.WebApiServices
{
    public class WebApiSpendTypeService : MvcWebApiBaseService, ISpendTypeService
    {
        #region Attributes

        protected override string ControllerName => "spendType";

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
			    throw new HttpException(response);
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

	    public WebApiSpendTypeService(IAppSettings appSettings) : base(appSettings)
	    {
	    }
    }
}