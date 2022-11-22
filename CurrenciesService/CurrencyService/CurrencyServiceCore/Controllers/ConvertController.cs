using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceModel;
using System.Threading.Tasks;
using System;
using MyFinanceModel.WebMethodsModel;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyServiceCore
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConvertController : ControllerBase
	{
		private readonly IDolarColonesBccrService _dolarColonesBccrService;

		public ConvertController(IDolarColonesBccrService dolarColonesBccrService)
		{
			_dolarColonesBccrService = dolarColonesBccrService;
		}

		[Route("ExchangeRateResultByMethodId")]
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

		[Route("ExchangeRateResultByMethodIds")]
		[HttpPost]
		public async Task<IEnumerable<ExchangeRateResult>> ExchangeRateResultByMethodIds([FromBody] ExchangeRateResultModel model)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));
			if (model.MethodIds == null || !model.MethodIds.Any())
				throw new ArgumentException("Cannot be null or empty", nameof(model));
			var methodIds = model.MethodIds;
			var dateTime = model.DateTime;
			var list = new List<ExchangeRateResult>();
			foreach (int i in methodIds.Where(i => !list.Exists(item => item.MethodId == i)))
			{
				list.Add(await GetExchangeRateResultAsync(i, dateTime));
			}
			return list;
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
	}
}
