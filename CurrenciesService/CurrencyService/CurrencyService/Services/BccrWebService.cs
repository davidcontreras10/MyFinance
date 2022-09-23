using CurrencyService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CurrencyService.wsindicadoreseconomicosSoap;
using Utilities;
using WebApiBaseConsumer;
using Ut = Utilities.SystemDataUtilities;

namespace CurrencyService.Services
{
	public class BccrWebService : WebApiBaseService
	{
		private readonly wsindicadoreseconomicosSoapClient _secureService = new wsindicadoreseconomicosSoapClient("wsindicadoreseconomicosSoap12");

		protected override string ControllerName => "";

		public async Task<IEnumerable<BccrSingleVentanillaModel>> GetBccrSingleVentanillaModelsAsync(string indicador, DateTime initial, DateTime end)
		{
			var dataTable = await GetIndicatorHttp(indicador, initial, end);
			if (dataTable == null)
			{
				return new List<BccrSingleVentanillaModel>();
			}
			return CreateBccrSingleVentanillaModel(dataTable);
		}

		private async Task<DataTable> GetIndicatorHttp(string indicator, DateTime initial, DateTime end)
		{
			var inicio = initial.ToString("dd/MM/yyyy");
			var final = end.ToString("dd/MM/yyyy");
			var url = CreateRootUrl(new Dictionary<string, object>
		{
			{ "Indicador", indicator },
			{ "FechaInicio", inicio },
			{ "FechaFinal", final },
			{ "Nombre", "david" },
			{ "SubNiveles", "s" },
			{ "CorreoElectronico", "dcontre10@gmail.com" },
			{ "Token", "1IO9ALGN0O" }
		});
			ServicePointManager
					.ServerCertificateValidationCallback +=
				(sender, cert, chain, sslPolicyErrors) => true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			var request = new WebApiRequest(url, HttpMethod.Get)
			{
				Headers = new Dictionary<string, string>
				{
					{"User-Agent", "PostmanRuntime/7.29.2"},
					{"Connection", "close"},
					{"Accept", "*/*"},
					{"Cache-Control", "no-cache"},
					{"Access-Control-Allow-Origin", "*"}
				}
			};
			var response = await GetResponseAsync(request);
			var jsonResponse = await response.Content.ReadAsStringAsync();
			jsonResponse = HttpUtility.HtmlDecode(jsonResponse);
			var reader = new StringReader(jsonResponse);
			var theDataSet = new DataSet();
			theDataSet.ReadXml(reader);
			return theDataSet.Tables.Count > 1 ? theDataSet.Tables[1] : null;
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
			return "https://181.193.30.41/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx/ObtenerIndicadoresEconomicosXML";
		}
	}
}