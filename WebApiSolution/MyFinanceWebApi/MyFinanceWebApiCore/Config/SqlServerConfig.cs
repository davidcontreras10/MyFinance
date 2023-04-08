using DataAccess;
using Microsoft.Extensions.Configuration;

namespace MyFinanceWebApiCore.Config
{
	public class SqlServerConfig : IConnectionConfig
	{
		private readonly IConfiguration _configuration;

		public SqlServerConfig(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.PersistentConnection;
		public string GetConnectionString()
		{
			return _configuration.GetConnectionString("SqlServerLocalConnection");
		}
	}
}
