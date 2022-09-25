using System;
using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;
using CurrencyService.BccrServiceReference;

namespace CurrencyService.Services
{
	public class BccrSoapWebRepository : IBccrCurrencyRepository
	{
		private readonly wsindicadoreseconomicosSoapClient _soapClient;

		public BccrSoapWebRepository()
		{
			try
			{
				_soapClient = new wsindicadoreseconomicosSoapClient("wsindicadoreseconomicosSoap12");
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.TraceError($"BccrSoapWebRepository Constructor Error: \n{e}");
				throw;
			}

			System.Diagnostics.Trace.TraceWarning($"BccrSoapWebRepository Constructor OK");
		}

		public async Task<DataTable> GetIndicatorAsync(string indicator, DateTime initial, DateTime end)
		{
			try
			{
				if (_soapClient.State != CommunicationState.Opened)
				{
					_soapClient.Open();
				}

				var inicio = initial.ToString("dd/MM/yyyy");
				var final = end.ToString("dd/MM/yyyy");
				var dataSet = await _soapClient.ObtenerIndicadoresEconomicosAsync(indicator, inicio, final, "David", "s",
					"dcontre10@gmail.com", "1IO9ALGN0O");
				return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.TraceError($"BccrSoapWebRepository Constructor Error: \n{e}");
				throw;
			}
		}
	}
}