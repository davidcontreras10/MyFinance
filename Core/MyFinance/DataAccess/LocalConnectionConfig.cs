namespace MyFinance.DataAccess
{
	public interface IConnStringProvider
	{
		string GetConnectionString(string name);
	}

    public class LocalConnectionConfig : IConnectionConfig
    {
	    private readonly IConnStringProvider _connStringProvider;

	    public LocalConnectionConfig(IConnStringProvider connStringProvider)
	    {
		    _connStringProvider = connStringProvider;
	    }

	    public string GetConnectionString()
	    {
		    return _connStringProvider.GetConnectionString("SqlServerLocalConnection");
	    }

	    public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.PersistentConnection;
    }
}
