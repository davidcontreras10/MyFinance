using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceModel;
using System.Collections.Generic;
using System;
using MyFinanceWebApiCore.Authentication;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SpendsController : BaseApiController
	{
		#region Attributes

		private readonly ISpendsService _spendsService;

		#endregion

		#region Constructors

		public SpendsController(ISpendsService spendsService)
		{
			_spendsService = spendsService;
		}

		#endregion

		#region Action Methods

		[Route("actionResult")]
		[RequiresHeaderFilter(ServiceAppHeader.ServiceAppHeaderType.ApplicationModule)]
		[HttpGet]
		public SpendActionResult GetSpendActionResult([FromQuery] int spendId, [FromQuery] ResourceActionNames actionType)
		{
			var moduleValue = GetModuleNameValue();
			var result = _spendsService.GetSpendActionResult(spendId, actionType, moduleValue);
			return result;
		}

		[Route("confirmation")]
		[HttpPut]
		public IEnumerable<SpendItemModified> ConfirmPendingSpend([FromQuery] int spendId, [FromBody] DateTimeModel newDateTime)
		{
			if (spendId == 0)
			{
				throw new ArgumentException("Value cannot be zero", nameof(spendId));
			}

			if (newDateTime == null)
			{
				throw new ArgumentNullException(nameof(newDateTime));
			}

			var modifiedItems = _spendsService.ConfirmPendingSpend(spendId, newDateTime.NewDateTime);
			return modifiedItems;
		}

		[HttpPatch]
		public IEnumerable<SpendItemModified> EditSpend([FromQuery] int spendId, [FromBody] ClientEditSpendModel model)
		{
			if(spendId < 0)
			{
				throw new ArgumentException("Value should be greater than 0", nameof(spendId));
			}

			if(model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			model.SpendId = spendId;
			model.UserId = GetUserId();
			return _spendsService.EditSpend(model);
		}

		[Route("basic")]
		[HttpPost]
		public IEnumerable<ItemModified> AddBasicSpend([FromBody] ClientBasicTrxByPeriod clientBasicTrxByPeriod)
		{
			clientBasicTrxByPeriod.UserId = GetUserId();
			return _spendsService.AddBasicTransaction(clientBasicTrxByPeriod, TransactionTypeIds.Spend);
		}

		[Route("basic/income")]
		[HttpPost]
		public IEnumerable<ItemModified> AddBasicIncome([FromBody] ClientBasicTrxByPeriod clientBasicTrxByPeriod)
		{
			clientBasicTrxByPeriod.UserId = GetUserId();
			return _spendsService.AddBasicTransaction(clientBasicTrxByPeriod, TransactionTypeIds.Saving);
		}

		[HttpPost]
		public IEnumerable<ItemModified> AddSpendCurrency(ClientAddSpendModel clientAddSpendModel)
		{
			clientAddSpendModel.UserId = GetUserId();
			var result = _spendsService.AddSpend(clientAddSpendModel);
			return result;
		}

		[Route("income")]
		[HttpPost]
		public IEnumerable<ItemModified> AddIncome(ClientAddSpendModel clientAddSpendModel)
		{
			clientAddSpendModel.UserId = GetUserId();
			var result = _spendsService.AddIncome(clientAddSpendModel);
			return result;
		}

		[HttpDelete]
		public IEnumerable<ItemModified> DeleteSpend(int spendId)
		{
			var userId = GetUserId();
			var itemModifiedList = _spendsService.DeleteSpend(userId, spendId);
			return itemModifiedList;
		}

		[Route("add")]
		[HttpGet]
		public IEnumerable<AddSpendViewModel> GetAddSpendViewModel([FromQuery] int[] accountPeriodIds)
		{
			var userId = GetUserId();
			var addSpendViewModelList = _spendsService.GetAddSpendViewModel(accountPeriodIds, userId);
			return addSpendViewModelList;
		}


		[Route("edit")]
		[HttpGet]
		public IEnumerable<EditSpendViewModel> GetEditSpendViewModel(int accountPeriodId, int spendId)
		{
			var userId = GetUserId();
			var editSpendViewModelList = _spendsService.GetEditSpendViewModel(accountPeriodId, spendId,
				userId);
			return editSpendViewModelList;
		}

		#endregion
	}
}
