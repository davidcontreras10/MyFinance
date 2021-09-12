using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using MyFinanceModel;
using MyFinanceModel.Utilities;

namespace MyFinanceWebApp.Services
{
    public abstract class BaseService
    {
        protected string CallWebService(string serviceName, string methodName, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new Exception("Invalid service name");
            if (string.IsNullOrEmpty(methodName))
                throw new Exception("Invalid method name");
            string url = GetUrl(serviceName);
            WebServiceCaller caller = new WebServiceCaller(url, methodName) {Params = SetParameters(parameters)};
            caller.Invoke(false);
            return caller.GetResponseXml();
        }

        protected WebServiceResponse CallWebServiceWithResponse(string serviceName, string methodName, Dictionary<string, object> parameters)
        {
            var result = CallWebService(serviceName, methodName, parameters);
            var webServiceResponse = string.IsNullOrEmpty(result)
                                            ? new WebServiceResponse()
                                            : XmlConvertion.DeserializeToXml<WebServiceResponse>(result);
            return webServiceResponse;
        }

        protected T CallWebServiceWithResponseValidate<T>(string serviceName, string methodName, Dictionary<string, object> parameters)
        {
            WebServiceResponse webServiceResponse = CallWebServiceWithResponse(serviceName, methodName, parameters);
            if (!webServiceResponse.IsValidResponse)
                throw new Exception(webServiceResponse.ErrorInfo);
            return XmlConvertion.DeserializeToXml<T>(webServiceResponse.Result);
        }

        private static string GetUrl(string serviceName)
        {
            string serverUrl = WebConfigurationManager.AppSettings["MyFinanceWsServer"];
            return serverUrl + serviceName;
        }

        private static Dictionary<string, string> SetParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null)
                return new Dictionary<string, string>();
            Dictionary<string, string> stringParameters=new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> keyValuePair in parameters.Where(item => item.Value != null))
            {
                string value = keyValuePair.Value is DateTime
                                   ? ((DateTime) keyValuePair.Value).ToString("s")
                                   : keyValuePair.Value.ToString();
                stringParameters.Add(keyValuePair.Key, value);
            }
            return stringParameters;
        } 
    }
}