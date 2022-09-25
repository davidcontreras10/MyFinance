using System;
using System.Data;
using System.Threading.Tasks;

namespace CurrencyService.Services
{
	public interface IBccrCurrencyRepository
	{
		Task<DataTable> GetIndicatorAsync(string indicator, DateTime initial, DateTime end);
	}
}
