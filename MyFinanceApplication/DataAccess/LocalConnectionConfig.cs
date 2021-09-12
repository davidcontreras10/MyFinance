using System.Configuration;

namespace DataAccess
{
    public class LocalConnectionConfig : IConnectionConfig
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SqlServerLocalConnection"].ConnectionString;
        }

	    public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.PersistentConnection;
    }
}
