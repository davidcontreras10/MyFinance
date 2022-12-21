using System;
using System.Collections.Generic;
using System.Linq;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceWebApp.Contants;
using MyFinanceModel.WebMethodsModel;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel.ViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
    public class WebApiSpendService : MvcWebApiBaseService, ISpendService
    {
        #region Legacy 

        public void AddPeriod(string token, int accountId, DateTime initial, DateTime end, float budget)
        {
            if (accountId == 0 || budget == 0 || initial >= end)
            {
                throw new Exception("Invalid parameters");
            }
            var model = new AddPeriodModel
            {
                AccountId = accountId,
                Budget = budget,
                End = end,
                Initial = initial,
            };
            var url = CreateMethodUrl(WebServicesConstants.ADD_PERIOD_METHOD);
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = model
            };

            GetResponse(request);
        }

        public AddPeriodData GetAddPeriodData(string token, int accountId, string userId)
        {
            if (string.IsNullOrEmpty(token) || accountId == 0)
            {
                throw new Exception("Invalid parameters");
            }
            var parameters = new Dictionary<string, object>
                {
                    {"userId", userId},
                    {"accountId", accountId}
                };
            var url = CreateMethodUrl(WebServicesConstants.GET_ADD_PERIOD_DATA_METHOD, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var addPeriodData = GetResponseAs<AddPeriodData>(request);
            return addPeriodData ?? new AddPeriodData();
        }

        public DateRange GetDateRange(string accountIds, DateTime? dateTime, string token, string userId)
        {
            var model = new GetDateRangeModel
            {
                AccountIds = accountIds,
                Date = dateTime ?? new DateTime(),
                DateSpecified = dateTime != null,
                UserId = userId
            };
            var url = CreateMethodUrl(WebServicesConstants.GET_DATE_RANGE);
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = model
            };

            var dateRange = GetResponseAs<DateRange>(request);
            return dateRange ?? new DateRange();
        }

        public IEnumerable<SpendType> GetSpendTypes()
        {
            string url = CreateMethodUrl(WebServicesConstants.GET_SPEND_TYPES_METHOD);
            var request = new WebApiRequest(url, HttpMethod.Get, "");
            var spendTypes = GetResponseAs<IEnumerable<SpendType>>(request);
            return spendTypes ?? new List<SpendType>();
        }

        #endregion

        #region Web Api

		public SpendActionResult GetSpendActionResult(int spendId, ResourceActionNames actionType, ApplicationModules applicationModule, string token)
		{
			var parameters = new Dictionary<string, object>
			{
				{"spendId", spendId},
				{nameof(actionType), actionType}
			};

			var appServiceHeader = ServiceAppHeader.GetServiceAppHeader(ServiceAppHeader.ServiceAppHeaderType.ApplicationModule);
			var headers = new Dictionary<string, string>
			{
				{appServiceHeader.Name, applicationModule.ToString()}
			};

			var url = CreateMethodUrl("actionResult", parameters);
			var request = new WebApiRequest(url, HttpMethod.Get, token)
			{
				Headers = headers
			};

			var response = GetResponseAs<SpendActionResult>(request);
			return response;
		}

		public IEnumerable<SpendItemModified> ConfirmPendingSpend(int spendId, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"spendId", spendId}
            };

            var model = new DateTimeModel
            {
                NewDateTime = DateTime.Now
            };

            var url = CreateMethodUrl("confirmation", parameters);
            var request = new WebApiRequest(url, HttpMethod.Put, token)
            {
                Model = model
            };

            var response = GetResponseAs<IEnumerable<SpendItemModified>>(request);
            return response;
        }

        public IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"spendId", model.SpendId}
            };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, new HttpMethod("PATCH"), token)
            {
                Model = model
            };

            var response = GetResponseAs<IEnumerable<SpendItemModified>>(request);
            return response;
        }

        public IEnumerable<EditSpendViewModel> GetEditSpendViewModel(IEnumerable<int> accountPeriodIds, string token, int spendId)
		{
			if (accountPeriodIds == null || !accountPeriodIds.Any())
				return new List<EditSpendViewModel>();
			var parameters = new Dictionary<string, object>
            {
                {"accountPeriodId", accountPeriodIds.First()},
                {"spendId", spendId}
            };

			var url = CreateMethodUrl("edit", parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var addSpendViewModelList = GetResponseAs<IEnumerable<EditSpendViewModel>>(request);
			return addSpendViewModelList;
		}

		public IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string token)
		{
			if (accountPeriodIds == null || !accountPeriodIds.Any())
				return new List<AddSpendViewModel>();
			var parameters = new Dictionary<string, object>
                {
                    {"accountPeriodIds", accountPeriodIds}
                };
			var url = CreateMethodUrl("add", parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var addSpendViewModelList = GetResponseAs<IEnumerable<AddSpendViewModel>>(request);
			return addSpendViewModelList;
		}

		public async Task<IEnumerable<AddSpendViewModel>> GetAddSpendViewModelAsync(
			IEnumerable<int> accountPeriodIds,
			string token
		)
		{
			if (accountPeriodIds == null || !accountPeriodIds.Any())
				return new List<AddSpendViewModel>();
			var parameters = new Dictionary<string, object>
			{
				{"accountPeriodIds", accountPeriodIds}
			};
			var url = CreateMethodUrl("add", parameters);
			var request = new WebApiRequest(url, HttpMethod.Get, token);
			var addSpendViewModelList = await GetResponseAsAsync<IEnumerable<AddSpendViewModel>>(request);
			return addSpendViewModelList;
		}

		public IEnumerable<ItemModified> AddSpendCurrency(string token, ClientAddSpendModel clientAddSpendModel)
        {
            if (clientAddSpendModel == null)
                throw new ArgumentNullException(nameof(clientAddSpendModel));
            var url = CreateMethodUrl();
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = clientAddSpendModel
            };

            var itemModifiedList = GetResponseAs<IEnumerable<ItemModified>>(request);
            return itemModifiedList;
        }

        public IEnumerable<ItemModified> AddIncome(string token, ClientAddSpendModel clientAddSpendModel)
        {
            if (clientAddSpendModel == null)
                throw new ArgumentNullException(nameof(clientAddSpendModel));
			var url = CreateMethodUrl("income");
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = clientAddSpendModel
            };

            var itemModifiedList = GetResponseAs<IEnumerable<ItemModified>>(request);
            return itemModifiedList;
        }

        public IEnumerable<ItemModified> DeleteSpend(string token, int spendId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"spendId", spendId}
                };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Delete, token);
            var response = GetResponseAs<IEnumerable<ItemModified>>(request);
            return response;
        }

		public async Task<IEnumerable<ItemModified>> AddBasicSpendAsync(string token, ClientBasicTrxByPeriod clientBasicTrxByPeriod)
		{
			if (clientBasicTrxByPeriod == null)
				throw new ArgumentNullException(nameof(clientBasicTrxByPeriod));
			var url = CreateMethodUrl("basic");
			var request = new WebApiRequest(url, HttpMethod.Post, token)
			{
				Model = clientBasicTrxByPeriod
			};

			return await GetResponseAsAsync<IEnumerable<ItemModified>>(request);
		}

	    public async Task<IEnumerable<ItemModified>> AddBasicIncomeAsync(string token, ClientBasicTrxByPeriod clientBasicTrxByPeriod)
	    {
		    if (clientBasicTrxByPeriod == null)
			    throw new ArgumentNullException(nameof(clientBasicTrxByPeriod));
		    var url = CreateMethodUrl("basic/income");
		    var request = new WebApiRequest(url, HttpMethod.Post, token)
		    {
			    Model = clientBasicTrxByPeriod
		    };

		    return await GetResponseAsAsync<IEnumerable<ItemModified>>(request);
	    }

		#endregion

		protected override string ControllerName => "spends";

		public WebApiSpendService(IHttpClientFactory httpClientFactory) : base(httpClientFactory, true)
		{
		}
    }
}