using CurrencyService.Services;
using MyFinanceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyFinanceModel.WebMethodsModel;

namespace CurrencyService.Controllers
{
    public class ConvertController : ApiController
    {
        #region Constructor

        public ConvertController()
        {
            _dolarColonesBccrService = new DolarColonesBccrService();
        }

        #endregion

        #region Attributes

        private readonly DolarColonesBccrService _dolarColonesBccrService;

        #endregion

        #region Actions

        [HttpGet]
        public ExchangeRateResult ExchangeRateResultByMethodId(int methodId, DateTime dateTime)
        {
            return GetExchangeRateResult(methodId, dateTime);
        }

        [HttpPost]
        public IEnumerable<ExchangeRateResult> ExchangeRateResultByMethodIds([FromBody]ExchangeRateResultModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (model.MethodIds == null || !model.MethodIds.Any())
                throw new ArgumentException("Cannot be null or empty", "model");
            var methodIds = model.MethodIds;
            var dateTime = model.DateTime;
            var list = new List<ExchangeRateResult>();
            foreach (int i in methodIds.Where(i => !list.Exists(item => item.MethodId == i)))
            {
                list.Add(GetExchangeRateResult(i, dateTime));
            }
            return list;
        }

        #endregion

        #region Private Methods

        private ExchangeRateResult GetExchangeRateResult(int methodId, DateTime dateTime)
        {
            if (methodId == 0)
            {
                throw new ArgumentException("Cannot be 0", "methodId");
            }

            var result = _dolarColonesBccrService.GetExchangeRateResultByMethodId(methodId, dateTime);
            result.MethodId = methodId;
            return result;
        }

        #endregion
    }
}