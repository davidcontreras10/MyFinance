using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
	public interface IBccrCurrencyRepository
	{
		Task<IEnumerable<BccrSingleVentanillaModel>> GetIndicatorAsync(string indicator, DateTime initial, DateTime end);
	}
}
