using System;

namespace MyFinance.MyFinanceModel
{
    public class WebServiceResponse
    {
        #region Constructor

        public WebServiceResponse()
        {
            Result = "";
            IsValidResponse = false;
            ExceptionObject = "";
            ErrorInfo = "Unknown error";
        }

        #endregion

        #region Methods

        public void SetToErrorState(Exception exception)
        {
            Result = "";
            IsValidResponse = false;
            ExceptionObject = exception.Message;
            ErrorInfo = exception.Message;
        }

        public void SetToValidState(string result)
        {
            Result = result;
            IsValidResponse = true;
            ExceptionObject = "";
            ErrorInfo = "";
        }

        #endregion

        #region Attributes

        public string Result { set; get; }
        public bool IsValidResponse { set; get; }
        public string ExceptionObject { set; get; }
        public string ErrorInfo { set; get; }

        #endregion
    }
}
