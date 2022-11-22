using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceModel;
using System.Threading.Tasks;
using System;

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
