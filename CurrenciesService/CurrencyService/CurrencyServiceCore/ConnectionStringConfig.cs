using DataAccess;
using Microsoft.Extensions.Configuration;

namespace CurrencyServiceCore
{
	public class ConnectionStringConfig : IConnectionConfig
	{
		private readonly IConfiguration _configuration;

		public ConnectionStringConfig(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.ConnectionPooling;

		public string GetConnectionString()
		{
			return _configuration.GetConnectionString("ExchangeCurrencyService");
		}
	}
}
