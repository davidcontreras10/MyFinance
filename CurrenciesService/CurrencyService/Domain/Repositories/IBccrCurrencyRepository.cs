using System;
using System.Data;
using System.Threading.Tasks;

namespace Domain.Repositories
{
	public interface IBccrCurrencyRepository
	{
		Task<DataTable> GetIndicatorAsync(string indicator, DateTime initial, DateTime end);
	}
}
