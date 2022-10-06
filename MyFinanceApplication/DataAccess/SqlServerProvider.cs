using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    internal static class SqlServerProvider
    {
        #region Private Attributes

        // ReSharper disable once InconsistentNaming
        private static readonly List<SqlConnection> _sqlConnections = new List<SqlConnection>();

        #endregion

        #region Private Class

        internal class ConnectionStringObject
        {
            public ConnectionStringObject(string connectionString)
            {
                if (string.IsNullOrEmpty(connectionString))
                    return;
                string[] connectionParts = connectionString.Split(';');
                foreach (string connectionPart in connectionParts)
                {
                    string part = connectionPart.Trim();
                    if (!string.IsNullOrEmpty(part))
                    {
                        string[] partParts = part.Split('=');
                        var name = partParts[0].Trim().ToLower();
                        var validServer = new[] {"server", "data source"};
                        var validDatabase = new[] {"database", "initial catalog"};
                        var value = partParts[1].Trim();
                        if (validServer.Any(vs=>vs==name))
                        {
                            Server = value;
                        }

                        if (validDatabase.Any(vs => vs == name))
                        {
                            Database = value;
                        }
                        if (name.ToLower() == "user id")
                        {
                            UserId = value;
                        }
                    }

                }
            }

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(Database) &&
                       !string.IsNullOrEmpty(UserId);
            }

            public string Server { get; set; }
            public string Database { set; get; }
            public string UserId { set; get; }
        }

        #endregion

        #region private methods

        internal static bool SameConnectionString(string s1, string s2)
        {
            if (s1 == null)
                s1 = "";
            if (s2 == null)
                s2 = "";
            if (!string.IsNullOrEmpty(s1) && s1 == s2)
            {
                return true;
            }

            var cs1 = new ConnectionStringObject(s1);
            var cs2 = new ConnectionStringObject(s2);
            if (!cs1.IsValid() || !cs2.IsValid())
            {
                throw new Exception("Unable to compare connection strings");
            }

            return cs1.Database == cs2.Database && cs1.Server == cs2.Server && cs1.UserId == cs2.UserId;
        }

        #endregion

        #region Public Methods

        public static void SetConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception($"Invalid Connection string {connectionString}");
            }
            SqlConnection sqlConnection =
                _sqlConnections.FirstOrDefault(item => SameConnectionString(item.ConnectionString, connectionString));
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connectionString);
                _sqlConnections.Add(sqlConnection);
            }
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
        }

        public static async Task<SqlConnection> GetSqlConnectionAsync(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception($"Invalid Connection string {connectionString}");
            }
            var sqlConnection =
                _sqlConnections.FirstOrDefault(item => SameConnectionString(item.ConnectionString, connectionString));
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connectionString);
                _sqlConnections.Add(sqlConnection);
            }
            if (sqlConnection.State != ConnectionState.Open)
            {
                try
                {
                    await sqlConnection.OpenAsync();
                }
                catch (Exception)
                {
                    _sqlConnections.Remove(sqlConnection);
                    sqlConnection = new SqlConnection(connectionString);
                    await sqlConnection.OpenAsync();
                    _sqlConnections.Add(sqlConnection);
                }
            }
            return sqlConnection;
        }

        public static SqlConnection GetSqlConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception($"Invalid Connection string {connectionString}");
            }
            var sqlConnection =
                _sqlConnections.FirstOrDefault(item => SameConnectionString(item.ConnectionString, connectionString));
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connectionString);
                _sqlConnections.Add(sqlConnection);
            }
            if (sqlConnection.State != ConnectionState.Open)
            {
                try
                {
                    sqlConnection.Open();
                }
                catch (Exception)
                {
                    _sqlConnections.Remove(sqlConnection);
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    _sqlConnections.Add(sqlConnection);
                }
            }
            return sqlConnection;
        }

        #endregion
    }
}
