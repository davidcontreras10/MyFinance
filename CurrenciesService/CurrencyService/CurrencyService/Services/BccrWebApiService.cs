using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WebApiBaseConsumer;

namespace CurrencyService.Services
{
	public class BccrWebApiService : WebApiBaseService, IBccrCurrencyRepository
	{

		protected override string ControllerName => string.Empty;

		//public async Task<DataTable> GetIndicatorAsync(string indicator, DateTime initial, DateTime end)
		//{
		//	var inicio = initial.ToString("dd/MM/yyyy");
		//	var final = end.ToString("dd/MM/yyyy");
		//	var postParams = new Dictionary<string, string>
		//	{
		//		{"Indicador", indicator},
		//		{"FechaInicio", inicio},
		//		{"FechaFinal", final},
		//		{"Nombre", "david"},
		//		{"SubNiveles", "s"},
		//		{"CorreoElectronico", "dcontre10@gmail.com"},
		//		{"Token", "1IO9ALGN0O"}
		//	};
		//	ServicePointManager
		//			.ServerCertificateValidationCallback +=
		//		(sender, cert, chain, sslPolicyErrors) => true;
		//	var postModel = new FormUrlEncodedContent(postParams);
		//	var request = new WebApiRequest(string.Empty, HttpMethod.Get)
		//	{
		//		Headers = new Dictionary<string, string>
		//		{
		//			{"User-Agent", "PostmanRuntime/7.29.2"},
		//			{"Connection", "close"},
		//			{"Accept", "*/*"},
		//			{"Cache-Control", "no-cache"},
		//			{"Access-Control-Allow-Origin", "*"},
		//			//{"Content-Type", "application/x-www-form-urlencoded"}
		//		},
		//		Model = postModel,
		//		IsJsonModel = false,
		//		Method = HttpMethod.Post
		//	};
		//	var response = await GetResponseAsync(request);
		//	var jsonResponse = await response.Content.ReadAsStringAsync();
		//	jsonResponse = HttpUtility.HtmlDecode(jsonResponse);
		//	var reader = new StringReader(jsonResponse);
		//	var theDataSet = new DataSet();
		//	theDataSet.ReadXml(reader);
		//	//var theDataSet = await response.Content.ReadAsAsync<DataSet>();
		//	return theDataSet.Tables.Count > 0? theDataSet.Tables[0] : null;
		//}

		public async Task<DataTable> GetIndicatorAsync(string indicator, DateTime initial, DateTime end)
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

		protected override string GetApiBaseDomain()
		{
			return "https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx/ObtenerIndicadoresEconomicos";
		}
	}
}