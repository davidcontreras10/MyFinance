using System;
using System.Collections.Generic;
using System.Web.Services;
using MyFinanceModel;
using MyFinanceModel.Utilities;

namespace MyFinanceWebService
{
    /// <summary>
    /// Summary description for MyFinanceAccountWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MyFinanceAccountWebService : WebService
    {
        #region Attributes

        private readonly BackendInstance _backendInstance;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public MyFinanceAccountWebService()
        {
            _backendInstance = new BackendInstance("SqlServerLocalConnection");
        }

        #endregion

        #region Web Methods

  

        #endregion

        
    }
}
