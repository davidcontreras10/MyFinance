using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.WebMethodsModel;

namespace MyFinance.Backend.Services
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
        private const string CurrencyServiceName = "Convert";
        private const string ConvertMethodByListName = "ExchangeRateResultByMethodIds";
        private const string ConvertMethodName = "ExchangeRateResultByMethodId";

        #endregion

        #region Constructors

        public CurrencyService(IMyFinanceSettings myFinanceSettings)
        {
            _serviceUrl = myFinanceSettings.CurrencyServiceUrl;
            if (string.IsNullOrEmpty(_serviceUrl))
                throw new Exception("CurrencyServiceUrl invalid");
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
            var methodUrl = CreateMethodUrl(ConvertMethodName, parameters);
            var result = GetResponseType<ExchangeRateResult>(methodUrl, false, null);
            return result;
        }

        private IEnumerable<ExchangeRateResult> GetExchangeRateResultService(IEnumerable<int> methodIds, DateTime dateTime)
        {
            var methodUrl = CreateMethodUrl(ConvertMethodByListName);
            var exchangeRateResultModel = new ExchangeRateResultModel
                {
                    DateTime = dateTime,
                    MethodIds = methodIds
                };
            var result = GetResponseType<IEnumerable<ExchangeRateResult>>(methodUrl, true, exchangeRateResultModel);
            return result;
        }

        protected override string GetApiBaseDomain()
        {
            return _serviceUrl;
        }

        protected override string ControllerName()
        {
            return CurrencyServiceName;
        }

        #endregion

    }
}
