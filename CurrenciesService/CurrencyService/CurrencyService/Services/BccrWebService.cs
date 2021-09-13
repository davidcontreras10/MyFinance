using CurrencyService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using CurrencyService.wsindicadoreseconomicosSoap;
using Utilities;
using WebApiBaseConsumer;
using Ut = Utilities.SystemDataUtilities;

namespace CurrencyService.Services
{
	public class BccrWebService : WebApiBaseService
	{
		private readonly wsindicadoreseconomicosSoapClient _secureService = new wsindicadoreseconomicosSoapClient("wsindicadoreseconomicosSoap12");

		protected override string ControllerName => "wsindicadoreseconomicos.asmx";

		public IEnumerable<BccrSingleVentanillaModel> GetBccrSingleVentanillaModels(string indicador, DateTime initial, DateTime end)
		{
			DataSet dataSet = GetIndicador(indicador, initial, end);
			if (dataSet == null || dataSet.Tables.Count < 1)
			{
				return new List<BccrSingleVentanillaModel>();
			}
			return CreateBccrSingleVentanillaModel(dataSet.Tables[0]);
		}

		private DataSet GetIndicatorHttp(string indicator, DateTime initial, DateTime end)
		{
			string inicio = initial.ToString("dd/MM/yyyy");
			string final = end.ToString("dd/MM/yyyy");
			string url = CreateRootUrl(new Dictionary<string, object>
		{
			{ "Indicador", indicator },
			{ "FechaInicio", inicio },
			{ "FechaFinal", final },
			{ "Nombre", "david" },
			{ "SubNiveles", "s" },
			{ "CorreoElectronico", "dcontre10@gmail.com" },
			{ "Token", "1IO9ALGN0O" }
		});
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			return GetResponse(new WebApiRequest(url, HttpMethod.Get)).Content.ReadAsAsync<DataSet>().Result;
		}

		private DataSet GetIndicador(string indicador, DateTime initial, DateTime end)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			string inicio = initial.ToString("dd/MM/yyyy");
			string final = end.ToString("dd/MM/yyyy");
			return _secureService.ObtenerIndicadoresEconomicos(indicador, inicio, final, "david", "s", "dcontre10@gmail.com", "1IO9ALGN0O");
		}

		private IEnumerable<BccrSingleVentanillaModel> CreateBccrSingleVentanillaModel(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return new List<BccrSingleVentanillaModel>();
			}
			IEnumerable<DataRow> enumerable = dataTable.Rows.Cast<DataRow>();
			List<BccrSingleVentanillaModel> list = new List<BccrSingleVentanillaModel>();
			foreach (DataRow row in enumerable)
			{
				list.Add(CreateBccrSingleVentanillaModel(row));
			}
			return list;
		}

		private BccrSingleVentanillaModel CreateBccrSingleVentanillaModel(DataRow dataRow)
		{
			if (dataRow == null)
			{
				throw new ArgumentNullException("dataRow");
			}
			float value = Ut.GetFloat(dataRow, "NUM_VALOR");
			DateTime lastUpdate = SystemDataUtilities.GetDateTime(dataRow, "DES_FECHA");
			return new BccrSingleVentanillaModel
			{
				LastUpdate = lastUpdate,
				Value = value
			};
		}

		protected override string GetApiBaseDomain()
		{
			return "https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS";
		}
	}
}