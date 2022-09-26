using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services
{
	public interface IExchangeCurrencyDataService
	{
		Task<IEnumerable<BccrVentanillaModel>> GetBccrVentanillaModelAsync(string entityName, DateTime dateTime);
		EntityMethodInfo GetEntityMethodInfo(int methodId);
	}
}
