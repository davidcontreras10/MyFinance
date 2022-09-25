using CurrencyService.Services;
using MyFinanceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HtmlAgilityPack;
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
        public string CheckAccess(string url)
        {
            return CheckUrl(url);
        }

        [HttpGet]
        public async Task<ExchangeRateResult> ExchangeRateResultByMethodId(int methodId, DateTime dateTime)
        {
	        System.Diagnostics.Trace.TraceInformation("ExchangeRateResultByMethodId called Log");
	        try
	        {
		        return await GetExchangeRateResultAsync(methodId, dateTime);
			}
	        catch (Exception e)
	        {

		        System.Diagnostics.Trace.TraceError($"Exchange Error:{Environment.NewLine}{e}");
		        throw;
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

        private string CheckHtmlDoc(string url)
        {
	        try
	        {
		        var web = new HtmlWeb();
		        var doc = web.Load(url);
		        return doc.ParsedText;
	        }
	        catch (Exception e)
	        {
		        System.Diagnostics.Trace.TraceError(e.ToString());
		        return e.ToString();
	        }

        }

        private string CheckUrl(string url)
        {
	        var urlCheck = new Uri(url);
	        var request = (HttpWebRequest) WebRequest.Create(urlCheck);
	        request.Timeout = 60 * 1000;
	        try
	        {
		        var response = request.GetResponse();
		        var encoding = Encoding.ASCII;
		        System.Diagnostics.Trace.TraceError("No Exception!");
		        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
		        {
			        var responseText = reader.ReadToEnd();
			        return responseText;
		        }
	        }
	        catch (Exception e)
	        {
		        System.Diagnostics.Trace.TraceError(e.ToString());
		        return e.ToString();
	        }
        }

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