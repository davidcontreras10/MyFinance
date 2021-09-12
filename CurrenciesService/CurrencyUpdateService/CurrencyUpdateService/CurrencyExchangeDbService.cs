using DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CurrencyUpdateService
{
    internal class CurrencyExchangeDbService : SqlServerBaseService
    {
        #region Constructor
        public CurrencyExchangeDbService(IConnectionConfig connectionConfig) : base(connectionConfig)
        {

        }

        #endregion

        #region Public Methods

        public void InsertBccrVentanillaModel(IEnumerable<BccrVentanillaModel> list)
        {
            try
            {
                var storedProcedure = "SpBccrVentanillaInsert";
                var dataTable = CreateBccrVentanillaModelTable(list);
                var tableParameter = new SqlParameter("@pValuesTable", dataTable);
                ExecuteStoredProcedure(storedProcedure, tableParameter);
            }
            catch(Exception ex)
            {

            }

        }

        #endregion

        #region Private Tables

        private static DataTable CreateBccrVentanillaModelTable(IEnumerable<BccrVentanillaModel> list)
        {
            var table = new DataTable("BccrVentanillaObject");
            table.Columns.Add("EntityName");
            table.Columns.Add("Purchase", new float().GetType());
            table.Columns.Add("Sell", new float().GetType());
            table.Columns.Add("LastUpdate", DateTime.Now.GetType());
            //table.Columns.Add("LastUpdate");
            if (list != null)
            {
                foreach(var model in list)
                {
                    table.Rows.Add(model.EntityName, model.Purchase, model.Sell, model.LastUpdate);
                }
            }
            return table;
        }

        #endregion
    }
}