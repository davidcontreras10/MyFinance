using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;


namespace MyFinanceWebApi.Helpers
{
	public class ConnectionConfig : IConnectionConfig
	{
		public string GetConnectionString()
		{
			return ConfigurationManager.ConnectionStrings["SqlServerLocalConnection"].ConnectionString;
		}

		public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.PersistentConnection;
	}
}
