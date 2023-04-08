using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using MyFinanceBackend.Models;
using MyFinanceModel;
using MyFinanceModel.WebMethodsModel;
using WebApiBaseConsumer;

namespace MyFinanceBackend.Services
{
	public interface ICurrencyService
	{
		ExchangeRateResult GetExchangeRateResult(int methodId, DateTime dateTime);
		IEnumerable<ExchangeRateResult> GetExchangeRateResult(IEnumerable<int> methodIds, DateTime dateTime);
	}

	public class CurrencyService : WebApiBaseService, ICurrencyService
	{
		#region Attributes

		private readonly string _serviceUrl;
		private const string CURRENCY_SERVICE_NAME = "Convert";
		private const string CONVERT_METHOD_BY_LIST_NAME = "ExchangeRateResultByMethodIds";
		private const string CONVERT_METHOD_NAME = "ExchangeRateResultByMethodId";

		protected override string ControllerName => CURRENCY_SERVICE_NAME;

		#endregion

		#region Constructors

		public CurrencyService(IHttpClientFactory httpClientFactory, IBackendSettings backendSettings) : base(httpClientFactory)
		{
			_serviceUrl = backendSettings.CurrencyServiceUrl;
		}

		#endregion

		#region Public Methods

		public ExchangeRateResult GetExchangeRateResult(int methodId, DateTime dateTime)
		{
			return GetExchangeRateResultService(methodId, dateTime);
		}

		public IEnumerable<ExchangeRateResult> GetExchangeRateResult(IEnumerable<int> methodIds, DateTime dateTime)
		{
			return methodIds == null || !methodIds.Any()
				? new List<ExchangeRateResult>()
				: GetExchangeRateResultService(methodIds, dateTime);
		}

		#endregion

		#region Private Methods

		private ExchangeRateResult GetExchangeRateResultService(int methodId, DateTime dateTime)
		{
			var requestDateTime = dateTime.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			var parameters = new Dictionary<string, object> { { "methodId", methodId }, { "dateTime", requestDateTime } };
			var methodUrl = CreateMethodUrl(CONVERT_METHOD_NAME, parameters);
			var request = new WebApiRequest(methodUrl, HttpMethod.Get);
			return GetResponseAs<ExchangeRateResult>(request);
		}

		private IEnumerable<ExchangeRateResult> GetExchangeRateResultService(IEnumerable<int> methodIds, DateTime dateTime)
		{
			var methodUrl = CreateMethodUrl(CONVERT_METHOD_BY_LIST_NAME);
			var exchangeRateResultModel = new ExchangeRateResultModel
				{
					DateTime = dateTime,
					MethodIds = methodIds
				};
			var request = new WebApiRequest(methodUrl, HttpMethod.Post)
			{
				Model = exchangeRateResultModel
			};

			return GetResponseAs<IEnumerable<ExchangeRateResult>>(request);
		}

		protected override string GetApiBaseDomain()
		{
			return _serviceUrl;
		}
		

		#endregion
	}
}
