using System;

namespace MyFinance.MyFinanceWebApp.CustomHandlers
{
    public class BackendTokenException : Exception
    {
        #region Attributes

        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }

        #endregion
    }
}