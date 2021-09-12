using System;
using System.Collections.Generic;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.Utilities;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.App_Utils;
using MyFinanceWebApp.Contants;


namespace MyFinanceWebApp.Services
{
    public class SpendService:BaseService, ISpendService
    {
        #region Methods

        public IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model, string token)
        {
            throw new NotImplementedException();
        }

        public DateRange GetDateRange(string accountIds, DateTime? dateTime, string token)
        {
            bool dateSpecified = true;
            if (dateTime == null)
            {
                dateSpecified = false;
                dateTime = new DateTime();
            }
            var parameters = new Dictionary<string, object>
                {
                    {"accountIds", accountIds},
                    {"dateTime", dateTime},
                    {"dateSpecified", dateSpecified ? 1 : 0},
                    {"userId", token}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME, WebServicesConstants.GET_DATE_RANGE,
                                           parameters);
            DateRange dateRange = XmlConvertion.DeserializeToXml<DateRange>(result);
            if (dateRange !=null && dateRange.EndDate != null)
            {
                DateTime endDate = (DateTime) dateRange.EndDate;
                endDate = InternalUtilities.FixEndDate(endDate);
                dateRange.EndDate = endDate;
            }
            return dateRange;
        }

        public AddPeriodData GetAddPeriodData(string token, int accountId)
        {
            if (string.IsNullOrEmpty(token) || accountId == 0)
            {
                throw new Exception("Invalid parameters");
            }
            var parameters = new Dictionary<string, object>
                {
                    {"userId", token},
                    {"accountId", accountId}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                               WebServicesConstants.GET_ADD_PERIOD_DATA_METHOD, parameters);
            return string.IsNullOrEmpty(result)
                       ? new AddPeriodData {AccountId = accountId}
                       : XmlConvertion.DeserializeToXml<AddPeriodData>(result);
        }

        public void AddPeriod(string userId, int accountId, DateTime initial, DateTime end, float budget)
        {
            if (string.IsNullOrEmpty(userId) || accountId == 0 || budget == 0 || initial >= end)
            {
                throw new Exception("Invalid parameters");
            }
            var parameters = new Dictionary<string, object>
                {
                    {"userId", userId},
                    {"accountId", accountId},
                    {"initial", initial},
                    {"end", end},
                    {"budget", budget}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                                           WebServicesConstants.ADD_PERIOD_METHOD, parameters);
            if (string.IsNullOrEmpty(result)) 
                return;
            string errorMessage = XmlConvertion.DeserializeToXml<string>(result);
            throw new Exception(errorMessage);
        }

	    public IEnumerable<EditSpendViewModel> GetEditSpendViewModel(IEnumerable<int> accountPeriodIds, string token, int spendId)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string token)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<ItemModified> AddSpendCurrency(ClientAddSpendModel clientAddSpendModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemModified> AddIncome(ClientAddSpendModel clientAddSpendModel)
        {
            throw new NotImplementedException();
        }

        public UserAccountsViewModel GetMainViewModel(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Invalid userId");
            var parameters = new Dictionary<string, object>
                {
                    {"userId", userId}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                                           WebServicesConstants.GET_MAIN_VIEW_METHOD, parameters);
            return string.IsNullOrEmpty(result) ? null : XmlConvertion.DeserializeToXml<UserAccountsViewModel>(result);
        }

        public IEnumerable<Spend> GetSpendList(string userId, int? spendTypeId, DateTime? startDate, DateTime? endTime)
        {
            var parameters = new Dictionary<string, object> { { "userId", userId } };
            if (spendTypeId != null)
            {
                parameters.Add("spendTypeId", spendTypeId);
            }
            if (startDate != null)
            {
                parameters.Add("startDate", startDate);
            }
            if (endTime != null)
            {
                parameters.Add("endTime", endTime);
            }
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                                           WebServicesConstants.GET_SPENDS_INFO_METHOD,
                                           parameters);
            return string.IsNullOrEmpty(result)
                       ? new List<Spend>()
                       : XmlConvertion.DeserializeToXml<List<Spend>>(result);
        }

        public IEnumerable<ItemModified> DeleteSpend(string token, int spendId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"userId", token},
                    {"spendId", spendId}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                               WebServicesConstants.DELETE_SPEND_METHOD,
                               parameters);
            WebServiceResponse webServiceResponse = string.IsNullOrEmpty(result)
                                                        ? new WebServiceResponse()
                                                        : XmlConvertion.DeserializeToXml<WebServiceResponse>(result);
            if (webServiceResponse.IsValidResponse)
            {
                throw new NotImplementedException();
            }
            //Exception exception = string.IsNullOrEmpty(webServiceResponse.ExceptionObject)
            //                          ? new Exception(webServiceResponse.ErrorInfo)
            //                          : XmlConvertion.DeserializeToXml<Exception>(webServiceResponse.ExceptionObject);
            Exception exception = new Exception(webServiceResponse.ErrorInfo);
            throw exception;
            
        }

        public void AddSpend(string userId, int spendType, DateTime date, double amount, string accountPeriodIds)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"userId", userId},
                    {"spendType", spendType},
                    {"date", date},
                    {"amount", amount},
                    {"accountPeriodIds", accountPeriodIds}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                               WebServicesConstants.ADD_SPEND_METHOD,
                               parameters);
            WebServiceResponse webServiceResponse = string.IsNullOrEmpty(result)
                                                        ? new WebServiceResponse()
                                                        : XmlConvertion.DeserializeToXml<WebServiceResponse>(result);
            if (webServiceResponse.IsValidResponse) 
                return;
            //Exception exception = string.IsNullOrEmpty(webServiceResponse.ExceptionObject)
            //                          ? new Exception(webServiceResponse.ErrorInfo)
            //                          : XmlConvertion.DeserializeToXml<Exception>(webServiceResponse.ExceptionObject);
            Exception exception = new Exception(webServiceResponse.ErrorInfo);
            throw exception;
        }

        public void AddSpendByAccounts(string userId, int spendType, DateTime date, double amount, string accountIds)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"userId", userId},
                    {"spendType", spendType},
                    {"date", date},
                    {"amount", amount},
                    {"accountIds", accountIds}
                };
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                               WebServicesConstants.ADD_SPEND_BY_ACCOUNT_METHOD,
                               parameters);
            WebServiceResponse webServiceResponse = string.IsNullOrEmpty(result)
                                                        ? new WebServiceResponse()
                                                        : XmlConvertion.DeserializeToXml<WebServiceResponse>(result);
            if (webServiceResponse.IsValidResponse)
                return;
            //Exception exception = string.IsNullOrEmpty(webServiceResponse.ExceptionObject)
            //                          ? new Exception(webServiceResponse.ErrorInfo)
            //                          : XmlConvertion.DeserializeToXml<Exception>(webServiceResponse.ExceptionObject);
            Exception exception = new Exception(webServiceResponse.ErrorInfo);
            throw exception;
        }

        public IEnumerable<SpendType> GetSpendTypes()
        {
            string result = CallWebService(WebServicesConstants.SPEND_SERVICE_NAME,
                                           WebServicesConstants.GET_SPEND_TYPES_METHOD,
                                           null);
            IEnumerable<SpendType> spendTypes = XmlConvertion.DeserializeToXml<List<SpendType>>(result);
            return spendTypes;
        }

        #endregion


        public IEnumerable<ItemModified> AddSpendCurrency(string token, ClientAddSpendModel clientAddSpendModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemModified> AddIncome(string token, ClientAddSpendModel clientAddSpendModel)
        {
            throw new NotImplementedException();
        }

        public AddPeriodData GetAddPeriodData(string token, int accountId, string userId)
        {
            throw new NotImplementedException();
        }

        public DateRange GetDateRange(string accountIds, DateTime? dateTime, string token, string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SpendItemModified> ConfirmPendingSpend(int spendId, string token)
        {
            throw new NotImplementedException();
        }

		public SpendActionResult GetSpendActionResult(int spendId, ResourceActionNames actionType, ApplicationModules applicationModule, string token)
		{
			throw new NotImplementedException();
		}
	}
}