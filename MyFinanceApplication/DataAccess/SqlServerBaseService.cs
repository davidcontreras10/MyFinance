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

        protected async Task<DataSet> ExecuteStoredProcedureAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            return await ExecuteStoredProcedureAsync(storedProcedure, parameters.ToList());
        }

        protected async Task<DataSet> ExecuteStoredProcedureAsync(string storedProcedure, List<SqlParameter> parameters)
        {
            if (_connectionAdminMode == ConnectionAdminModes.ConnectionPooling)
            {
                return await ExecuteStoredProcedureConnectionPoolingAsync(storedProcedure, parameters);
            }

            if (_connectionAdminMode == ConnectionAdminModes.PersistentConnection)
            {
                return await ExecuteStoredProcedurePersistentConnectionAsync(storedProcedure, parameters);
            }

            throw new Exception("Invalid connection mode");
        }

        private DataSet ExecuteStoredProcedurePersistentConnection(
            string storedProcedure,
            IEnumerable<SqlParameter> parameters
        )
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

        private async Task<DataSet> ExecuteStoredProcedurePersistentConnectionAsync(
            string storedProcedure,
            IEnumerable<SqlParameter> parameters
        )
        {
            var ds = new DataSet();
            using (var sqlCommand = new SqlCommand(storedProcedure)
            {
                Connection = await GetSqlConnectionAsync(),
                CommandType = CommandType.StoredProcedure,
                Transaction = _sqlTransaction
            })
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.Add(parameter);
                }

                using (var sqlDataAdapter = new AsyncDataAdapter.SqlDataAdapter(sqlCommand))
                {
                    await sqlDataAdapter.FillAsync(ds);
                }

            }

            return ds;
        }

        private DataSet ExecuteStoredProcedureConnectionPooling(
            string storedProcedure,
            IEnumerable<SqlParameter> parameters
        )
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

        private async Task<DataSet> ExecuteStoredProcedureConnectionPoolingAsync(
            string storedProcedure,
            IEnumerable<SqlParameter> parameters
        )
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

                    using (var sqlDataAdapter = new AsyncDataAdapter.SqlDataAdapter(sqlCommand))
                    {
                        await sqlDataAdapter.FillAsync(dataSet);
                    }
                }
            }

            return dataSet;
        }

        private async Task<SqlConnection> GetSqlConnectionAsync()
        {
            return await SqlServerProvider.GetSqlConnectionAsync(_connectionString);
        }
    }
}
