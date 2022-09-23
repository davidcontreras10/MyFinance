using CurrencyService.Services;
using MyFinanceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        public async Task<ExchangeRateResult> ExchangeRateResultByMethodId(int methodId, DateTime dateTime)
        {
	        try
	        {
		        return await GetExchangeRateResultAsync(methodId, dateTime);
	        }
	        catch (Exception e)
	        {
		        try
		        {
			        var hostInfo = Dns.GetHostByName("gee.bccr.fi.cr");
			        var ips = hostInfo.AddressList.Aggregate(string.Empty, (current, ipAddress) => current + $"Ip: {ipAddress}, ");
			        return new ExchangeRateResult
			        {
				        ErrorDetails = ips
			        };
		        }
		        catch (Exception exception)
		        {
			        Console.WriteLine(exception);
			        return new ExchangeRateResult
			        {
				        ErrorDetails = exception.ToString()
			        };
		        }

		        Console.WriteLine(e);
		        return new ExchangeRateResult
		        {
			        ErrorDetails = e.ToString()
		        };
	        }
        }

        [HttpPost]
        public async Task<IEnumerable<ExchangeRateResult>> ExchangeRateResultByMethodIds([FromBody]ExchangeRateResultModel model)
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
                list.Add(await GetExchangeRateResultAsync(i, dateTime));
            }
            return list;
        }

        #endregion

        #region Private Methods

        private async Task<ExchangeRateResult> GetExchangeRateResultAsync(int methodId, DateTime dateTime)
        {
            if (methodId == 0)
            {
                throw new ArgumentException("Cannot be 0", "methodId");
            }

            var result = await _dolarColonesBccrService.GetExchangeRateResultByMethodIdAsync(methodId, dateTime);
            result.MethodId = methodId;
            return result;
        }

        #endregion
    }
}