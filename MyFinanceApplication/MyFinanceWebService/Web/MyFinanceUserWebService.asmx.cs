using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MyFinanceModel;
using MyFinanceModel.Utilities;

namespace MyFinanceWebService
{
    /// <summary>
    /// Summary description for MyFinanceUserWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MyFinanceUserWebService : WebService
    {

        private readonly BackendInstance _backendInstance;

        /// <summary>
        /// 
        /// </summary>
        public MyFinanceUserWebService()
        {
            _backendInstance = new BackendInstance("SqlServerLocalConnection");
        }

        #region Web Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [WebMethod]
        public string ResultLoginAttempt(string username, string password)
        {
            LoginResult loginResult = _backendInstance.Users.AttemptToLogin(username, password);
            return XmlConvertion.SerializeToXml(loginResult);
        }

        #endregion

    }
}
