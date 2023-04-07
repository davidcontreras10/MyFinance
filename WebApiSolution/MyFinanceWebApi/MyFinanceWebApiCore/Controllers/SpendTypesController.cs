using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System.Collections.Generic;
using System;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SpendTypesController : BaseApiController
	{
		#region Attributes

		private readonly ISpendTypeService _spendTypeService;

		#endregion

		#region Constructor

		public SpendTypesController(ISpendTypeService spendTypeService)
		{
			_spendTypeService = spendTypeService;
		}

		#endregion

		#region Routes

		[HttpGet]
		public IEnumerable<SpendTypeViewModel> GetSpendTypes(bool includeAll = true)
		{
			var userId = GetUserId();
			var result = _spendTypeService.GeSpendTypes(userId, includeAll);
			return result;
		}

		[HttpDelete]
		public void DeleteSpendType([FromBody] ClientSpendTypeId clientSpendTypeId)
		{
			var userId = GetUserId();
			_spendTypeService.DeleteSpendType(userId, clientSpendTypeId.SpendTypeId);
		}

		[HttpPost]
		public IEnumerable<int> AddSpendType(ClientAddSpendType spendType)
		{
			if (spendType == null)
			{
				throw new ArgumentNullException("spendType");
			}

			var userId = GetUserId();
			spendType.SpendTypeId = 0;
			var result = _spendTypeService.AddEditSpendTypes(userId, spendType);
			return result;
		}

		[HttpPatch]
		public IEnumerable<int> EditSpendType(ClientEditSpendType spendType)
		{
			if (spendType == null)
			{
				throw new ArgumentNullException("spendType");
			}

			if (spendType.SpendTypeId < 1)
			{
				throw new ArgumentException("Id cannot be zero or less", "spendType");
			}

			var userId = GetUserId();
			var result = _spendTypeService.AddEditSpendTypes(userId, spendType);
			return result;
		}

		[Route("user")]
		[HttpPost]
		public IEnumerable<int> AddSpendTypeUser([FromBody] ClientSpendTypeId clientSpendTypeId)
		{
			var userId = GetUserId();
			var result = _spendTypeService.AddSpendTypeUser(userId, clientSpendTypeId.SpendTypeId);
			return result;
		}

		[Route("user")]
		[HttpDelete]
		public IEnumerable<int> DeleteSpendTypeUser([FromBody] ClientSpendTypeId clientSpendTypeId)
		{
			var userId = GetUserId();
			var result = _spendTypeService.DeleteSpendTypeUser(userId, clientSpendTypeId.SpendTypeId);
			return result;
		}

		#endregion
	}
}
