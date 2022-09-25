using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CurrencyService.Models;
using Domain.Repositories;
using Utilities;

namespace CurrencyService.Services
{
	public class BccrCurrencyService
	{
		private readonly IBccrCurrencyRepository _bccrCurrencyRepository;

		public BccrCurrencyService(IBccrCurrencyRepository bccrCurrencyRepository)
		{
			_bccrCurrencyRepository = bccrCurrencyRepository;
		}

		public async Task<IEnumerable<BccrSingleVentanillaModel>> GetBccrSingleVentanillaModelsAsync(string indicador, DateTime initial, DateTime end)
		{
			var dataTable = await _bccrCurrencyRepository.GetIndicatorAsync(indicador, initial, end);
			if (dataTable == null)
			{
				return new List<BccrSingleVentanillaModel>();
			}
			return CreateBccrSingleVentanillaModel(dataTable);
		}

		private IEnumerable<BccrSingleVentanillaModel> CreateBccrSingleVentanillaModel(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return new List<BccrSingleVentanillaModel>();
			}
			IEnumerable<DataRow> enumerable = dataTable.Rows.Cast<DataRow>();
			List<BccrSingleVentanillaModel> list = new List<BccrSingleVentanillaModel>();
			foreach (DataRow row in enumerable)
			{
				list.Add(CreateBccrSingleVentanillaModel(row));
			}
			return list;
		}

		private BccrSingleVentanillaModel CreateBccrSingleVentanillaModel(DataRow dataRow)
		{
			if (dataRow == null)
			{
				throw new ArgumentNullException("dataRow");
			}
			float value = SystemDataUtilities.GetFloat(dataRow, "NUM_VALOR");
			DateTime lastUpdate = SystemDataUtilities.GetDateTime(dataRow, "DES_FECHA");
			return new BccrSingleVentanillaModel
			{
				LastUpdate = lastUpdate,
				Value = value
			};
		}
	}
}