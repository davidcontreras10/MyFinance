
using System;
using System.Collections.Generic;
using System.Web.Services;
using MyFinanceModel;
using MyFinanceModel.Utilities;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebService
{
    /// <summary>
    /// Summary description for MyFinanceSpendWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MyFinanceSpendWebService : WebService
    {

        #region Constructor

        public MyFinanceSpendWebService()
        {
            _backendInstance = new BackendInstance("SqlServerLocalConnection");
        }

        #endregion

        #region Attributes

        private readonly BackendInstance _backendInstance;

        #endregion

        #region Web Methods

        [WebMethod]
        public string EditSpend(int spendId, string userId, string modifyList, DateTime spendDate, float amount,
                                string accountIds, string description, int spendTypeId)
        {
            WebServiceResponse response=new WebServiceResponse();
            try
            {
                _backendInstance.Spends.EditSpend(spendId, userId, modifyList, spendDate, amount, accountIds,
                                                  description, spendTypeId);
                response.SetToValidState("");
            }
            catch (Exception ex)
            {
                response.SetToErrorState(ex);
            }
            return XmlConvertion.SerializeToXml(response);
        }

        [WebMethod]
        public string GetDateRange(string accountIds, DateTime dateTime, int dateSpecified, string userId)
        {
            DateTime? dateValue = null;
            if (dateSpecified == 1)
                dateValue = dateTime;
            DateRange dateRange = _backendInstance.Spends.GetDateRange(accountIds, dateValue, userId);
            string result = XmlConvertion.SerializeToXml(dateRange);    
            return result;
        }


        [WebMethod]
        public string GetAddPeriodData(string userId, int accountId)
        {
            AddPeriodData addPeriodData = _backendInstance.AccountPeriods.GetAddPeriodData(accountId, userId);
            string result = XmlConvertion.SerializeToXml(addPeriodData);
            return result;
        }

        [WebMethod]
        public string AddPeriod(string userId, int accountId, DateTime initial, DateTime end, float budget)
        {
            string result = "";
            try
            {
                _backendInstance.AccountPeriods.CreateAccountPeriod(userId, accountId, initial, end, budget);
            }
            catch (Exception ex)
            {
                result = XmlConvertion.SerializeToXml(ex.Message);
            }
            return result;
        }

        [WebMethod]
        public string DeleteSpend(string userId, int spendId)
        {
            WebServiceResponse response = new WebServiceResponse();
            try
            {
                _backendInstance.Spends.DeleteSpend(userId, spendId);
                response.IsValidResponse = true;
            }
            catch (Exception ex)
            {
                response.IsValidResponse = false;
                //Exception exception = new Exception(ex.Message);
                //response.ExceptionObject = XmlConvertion.SerializeToXml(exception);
                response.ErrorInfo = ex.Message;
            }
            return XmlConvertion.SerializeToXml(response);
        }

        [WebMethod]
        public string AddSpend(string userId, int spendType, DateTime date, double amount, string accountPeriodIds)
        {
            WebServiceResponse response = new WebServiceResponse();
            try
            {
                //_backendInstance.Spends.AddSpend(userId, spendType, date, amount, accountPeriodIds);
                response.IsValidResponse = true;
            }
            catch (Exception ex)
            {
                response.IsValidResponse = false;
                //Exception exception = new Exception(ex.Message);
                //response.ExceptionObject = XmlConvertion.SerializeToXml(exception);
                response.ErrorInfo = ex.Message;
            }
            return XmlConvertion.SerializeToXml(response);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetMainViewModelData(string userId)
        {
            MainViewModel mainViewModel = _backendInstance.ViewData.GetMainViewModelData(userId);
            string result = XmlConvertion.SerializeToXml(mainViewModel);
            return result;
        }



        #endregion
    }
}
