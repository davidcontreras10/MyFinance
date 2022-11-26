using System.Configuration;
using System.Net.Http;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public abstract class MvcWebApiBaseService : WebApiBaseService
	{
		private bool _coreVersion = false;

		protected override string GetApiBaseDomain()
		{
			var appName = _coreVersion ? "MyFinanceCoreWsServer" : "MyFinanceWsServer";
			var domainValue = ConfigurationManager.AppSettings[appName];
			if (string.IsNullOrEmpty(domainValue))
				throw new ConfigurationErrorsException("MyFinanceWsServer value is not set");
			return domainValue;
		}

		protected MvcWebApiBaseService(IHttpClientFactory httpClientFactory, bool coreVersion = false) : base(httpClientFactory)
		{
			_coreVersion = coreVersion;
		}
	}
}


