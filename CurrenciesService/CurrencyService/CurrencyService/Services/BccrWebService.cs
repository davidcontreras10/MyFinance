using CurrencyService.BancoCentralServiceReference;
using CurrencyService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ut = Utilities.SystemDataUtilities;

namespace CurrencyService.Services
{
    public class BccrWebService
    {
        private wsIndicadoresEconomicosSoapClient _service = new wsIndicadoresEconomicosSoapClient();

        #region Public Methods

        public IEnumerable<BccrSingleVentanillaModel> GetBccrSingleVentanillaModels(string indicador, DateTime initial, DateTime end)
        {
            var dataSet = GetIndicador(indicador, initial, end);
            if (dataSet == null || dataSet.Tables.Count < 1)
                return new List<BccrSingleVentanillaModel>();
            return CreateBccrSingleVentanillaModel(dataSet.Tables[0]);
        }

        #endregion

        #region Private Methods
        
        private DataSet GetIndicador(string indicador,DateTime initial, DateTime end)
        {
            const string nombre = "dcontre10@gmail.com";
            const string subnivel = "s";
            const string formatDate = "dd/MM/yyyy";
            var inicio = initial.ToString(formatDate);
            var final = end.ToString(formatDate);
            var response = _service.ObtenerIndicadoresEconomicos(indicador, inicio, final, nombre, subnivel);
            return response;
        }

        private IEnumerable<BccrSingleVentanillaModel> CreateBccrSingleVentanillaModel(DataTable dataTable)
        {
            if (dataTable == null)
                return new List<BccrSingleVentanillaModel>();
            var rowList = dataTable.Rows.Cast<DataRow>();
            var list = new List<BccrSingleVentanillaModel>();
            foreach(var row in rowList)
            {
                list.Add(CreateBccrSingleVentanillaModel(row));
            }
            return list;
        }

        private BccrSingleVentanillaModel CreateBccrSingleVentanillaModel(DataRow dataRow)
        {
            if (dataRow == null)
                throw new ArgumentNullException("dataRow");
            var value = Ut.GetFloat(dataRow, "NUM_VALOR");
            var lastUpdate = Ut.GetDateTime(dataRow, "DES_FECHA");
            return new BccrSingleVentanillaModel
            {
                LastUpdate = lastUpdate,
                Value = value
            };
        }

        #endregion  
    }
}