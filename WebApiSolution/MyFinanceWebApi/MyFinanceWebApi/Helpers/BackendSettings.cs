using MyFinanceBackend.Models;
using System.Configuration;

namespace MyFinanceWebApi.Helpers
{
	public class BackendSettings : IBackendSettings
	{
		public string CurrencyServiceUrl => ConfigurationManager.AppSettings["CurrencyServiceUrl"];
	}
}
