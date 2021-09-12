namespace DataAccess
{
    public interface IConnectionConfig
    {
        string GetConnectionString();

	    ConnectionAdminModes ConnectionAdminMode { get; }
    }

	public enum ConnectionAdminModes
	{
		Unknown = 0,
		ConnectionPooling,
		PersistentConnection
	}
}