using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using Utilities;

namespace Domain.Services
{
	public interface IBccrCurrencyService
	{
		Task<IEnumerable<BccrSingleVentanillaModel>> GetBccrSingleVentanillaModelsAsync(string indicador, DateTime dateTime);
	}

	public class BccrCurrencyService : IBccrCurrencyService
	{
		private readonly IBccrCurrencyRepository _bccrCurrencyRepository;
		private readonly IBccrExchangeCache _bccrExchangeCache;

		public BccrCurrencyService(IBccrCurrencyRepository bccrCurrencyRepository, IBccrExchangeCache bccrExchangeCache)
		{
			_bccrCurrencyRepository = bccrCurrencyRepository;
			_bccrExchangeCache = bccrExchangeCache;
		}

		public async Task<IEnumerable<BccrSingleVentanillaModel>> GetBccrSingleVentanillaModelsAsync(string indicador, DateTime dateTime)
		{
			var cache = _bccrExchangeCache.Get(indicador, dateTime);
			if (cache != null)
			{
				return new[] { cache };
			}

			var results = await GetFromDbBccrSingleVentanillaModelsAsync(indicador, dateTime);
			if (results == null || !results.Any())
			{
				return Array.Empty<BccrSingleVentanillaModel>();
			}

			_bccrExchangeCache.Set(indicador, results);
			return results;
		}

		private async Task<IEnumerable<BccrSingleVentanillaModel>> GetFromDbBccrSingleVentanillaModelsAsync(string indicador, DateTime dateTime)
		{
			var initialDate = dateTime.AddMonths(-1);
			var endDate = dateTime.AddDays(1);
			var results = await _bccrCurrencyRepository.GetIndicatorAsync(indicador, initialDate, endDate);
			if (results == null || !results.Any())
			{
				return Array.Empty<BccrSingleVentanillaModel>();
			}

			return results;
		}
	}
}