using System.Configuration;
using System.Net;
using System.Net.Http;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public abstract class MvcWebApiBaseService : WebApiBaseService
	{
		public bool CoreVersion { private set; get; } = false;

		protected override string GetApiBaseDomain()
		{
			var appName = CoreVersion ? "MyFinanceCoreWsServer" : "MyFinanceWsServer";
			var domainValue = ConfigurationManager.AppSettings[appName];
			if (string.IsNullOrEmpty(domainValue))
				throw new ConfigurationErrorsException("MyFinanceWsServer value is not set");
			return domainValue;
		}

		protected MvcWebApiBaseService(IHttpClientFactory httpClientFactory, bool coreVersion = false) : base(httpClientFactory)
		{
			CoreVersion = coreVersion;
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}
	}
}


