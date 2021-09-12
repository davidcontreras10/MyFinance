using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SqlServerBaseService
    {
        private static SqlTransaction _sqlTransaction;
        private readonly string _connectionString;
	    private readonly ConnectionAdminModes _connectionAdminMode;

        private SqlConnection SqlConnection => SqlServerProvider.GetSqlConnection(_connectionString);

        public SqlServerBaseService(IConnectionConfig config)
        {
            _connectionString = config.GetConnectionString();
	        _connectionAdminMode = config.ConnectionAdminMode;
	        if (_connectionAdminMode == ConnectionAdminModes.PersistentConnection)
	        {
		        SqlServerProvider.SetConnection(_connectionString);
			}
        }

        protected void BeginTransaction()
        {
	        if (_connectionAdminMode != ConnectionAdminModes.PersistentConnection)
	        {
		        throw new Exception("Transactions supported only for persistent connections");
	        }

            _sqlTransaction = SqlConnection.BeginTransaction();
        }

        protected void RollbackTransaction()
        {
	        if (_connectionAdminMode != ConnectionAdminModes.PersistentConnection)
	        {
		        throw new Exception("Transactions supported only for persistent connections");
	        }

			if (_sqlTransaction != null)
            {
                _sqlTransaction.Rollback();
                _sqlTransaction.Dispose();
                _sqlTransaction = null;
            }
        }

        protected void Commit()
        {
	        if (_connectionAdminMode != ConnectionAdminModes.PersistentConnection)
	        {
		        throw new Exception("Transactions supported only for persistent connections");
	        }

			if (_sqlTransaction != null)
            {
                _sqlTransaction.Commit();
                _sqlTransaction.Dispose();
                _sqlTransaction = null;
            }
        }

        protected DataSet ExecuteStoredProcedure(string storedProcedure, params SqlParameter[] parameters)
        {
            return ExecuteStoredProcedure(storedProcedure, parameters.ToList());
        }

        protected DataSet ExecuteStoredProcedure(string storedProcedure, List<SqlParameter> parameters)
        {
	        if (_connectionAdminMode == ConnectionAdminModes.ConnectionPooling)
	        {
		        return ExecuteStoredProcedureConnectionPooling(storedProcedure, parameters);
	        }

	        if (_connectionAdminMode == ConnectionAdminModes.PersistentConnection)
	        {
		        return ExecuteStoredProcedurePersistentConnection(storedProcedure, parameters);
	        }

	        throw new Exception("Invalid connection mode");
        }

	    private DataSet ExecuteStoredProcedurePersistentConnection(string storedProcedure, IEnumerable<SqlParameter> parameters)
	    {
		    var ds = new DataSet();
		    using (var sqlCommand = new SqlCommand(storedProcedure)
		    {
			    Connection = SqlConnection,
			    CommandType = CommandType.StoredProcedure,
			    Transaction = _sqlTransaction
		    })
		    {
			    foreach (var parameter in parameters)
			    {
				    sqlCommand.Parameters.Add(parameter);
			    }

				using (var sqlDataAdapter = new SqlDataAdapter(sqlCommand))
				{
					sqlDataAdapter.Fill(ds);
				}

			}

		    return ds;
		}

	    private DataSet ExecuteStoredProcedureConnectionPooling(string storedProcedure, IEnumerable<SqlParameter> parameters)
	    {
		    var dataSet = new DataSet();
		    using (var sqlConnection = new SqlConnection(_connectionString))
		    {
			    using (var sqlCommand = new SqlCommand(storedProcedure)
			    {
				    Connection = sqlConnection,
				    CommandType = CommandType.StoredProcedure,
				    Transaction = _sqlTransaction
			    })
			    {
				    foreach (var parameter in parameters)
				    {
					    sqlCommand.Parameters.Add(parameter);
				    }

					using (var sqlDataAdapter = new SqlDataAdapter(sqlCommand))
					{
						sqlDataAdapter.Fill(dataSet);
					}
				}
			}

		    return dataSet;
	    }

	    private static void RunCommand(DataSet dataSet, SqlCommand sqlCommand)
	    {
		    using (var sqlDataAdapter = new SqlDataAdapter(sqlCommand))
		    {
				sqlDataAdapter.Fill(dataSet);
			}
		}

	    private static async Task RunCommandAsync(DataSet dataSet, SqlCommand sqlCommand)
	    {
		    await Task.Run(() => { RunCommand(dataSet, sqlCommand); });
	    }
	}
}
