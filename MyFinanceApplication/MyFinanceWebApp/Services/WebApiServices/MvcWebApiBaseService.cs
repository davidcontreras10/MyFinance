using System.Configuration;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
    public abstract class MvcWebApiBaseService : WebApiBaseService
	{
		protected override string GetApiBaseDomain()
		{
			var domainValue = ConfigurationManager.AppSettings["MyFinanceWsServer"];
			if (string.IsNullOrEmpty(domainValue))
				throw new ConfigurationErrorsException("MyFinanceWsServer value is not set");
			return domainValue;
		}
	}
}


