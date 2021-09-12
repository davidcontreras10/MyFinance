using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApi.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace MyFinanceWebApi.Controllers
{
	[RoutePrefix("api/spends")]
	public class SpendsController : BaseController
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
		public SpendActionResult GetSpendActionResult([FromUri]int spendId, [FromUri]ResourceActionNames actionType)
		{
			var moduleValue = GetModuleNameValue();
			var result = _spendsService.GetSpendActionResult(spendId, actionType, moduleValue);
			return result;
		}

		[Route("confirmation")]
		[HttpPut]
		public IEnumerable<SpendItemModified> ConfirmPendingSpend([FromUri]int spendId, [FromBody]DateTimeModel newDateTime)
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

		[Route]
		[HttpPatch]
		[AvoidModelStateValidation]
		public IEnumerable<SpendItemModified> EditSpend([FromUri] int spendId, [FromBody] ClientEditSpendModel model)
		{
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

		[Route]
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

		[Route]
		[HttpDelete]
		public IEnumerable<ItemModified> DeleteSpend(int spendId)
		{
			var userId = GetUserId();
			var itemModifiedList = _spendsService.DeleteSpend(userId, spendId);
			return itemModifiedList;
		}

		[Route("add")]
		[HttpGet]
		public IEnumerable<AddSpendViewModel> GetAddSpendViewModel([FromUri]int[] accountPeriodIds)
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

		#region Old Methods

		//[HttpPost]
		//public DateRange GetDateRange([FromBody]GetDateRangeModel model)
		//{
		//    DateTime? dateValue = null;
		//    if (model.DateSpecified)
		//        dateValue = model.Date;
		//    DateRange dateRange = _backendInstance.Spends.GetDateRange(model.AccountIds, dateValue, model.Username);
		//    return dateRange;
		//}

		//[HttpGet]
		//public SpendDetailSource GetSpendDetailData(int spendId, string username)
		//{
		//    SpendDetailSource spendDetailSource = _backendInstance.Spends.GetSpendDetailData(spendId, username);
		//    return spendDetailSource;
		//}

		//[HttpGet]
		//public AddPeriodData GetAddPeriodData(string username, int accountId)
		//{
		//    AddPeriodData addPeriodData = _backendInstance.AccountPeriods.GetAddPeriodData(accountId, username);
		//    return addPeriodData;
		//}

		//[HttpPost]
		//public void AddPeriod([FromBody]AddPeriodModel model)
		//{
		//    _backendInstance.AccountPeriods.CreateAccountPeriod(model.Username, model.AccountId, model.Initial, model.End, model.Budget);
		//}

		//[HttpPost]
		//public void DeleteSpend(string username, int spendId)
		//{
		//    _backendInstance.Spends.DeleteSpend(username, spendId);
		//}

		//[HttpPost]
		//public List<Spend> GetSpendsInfo([FromBody]GetSpendsInfoModel model)
		//{
		//    List<Spend> listSpend = _backendInstance.Spends.ListSpend(model.Username, model.SpendTypeId, model.StartDate, model.EndTime);
		//    return listSpend;
		//}

		//[HttpGet]
		//public SpendsByPeriod GetSpendsByPeriod(string username, string accountPeriodIds)
		//{
		//    SpendsByPeriod spends = _backendInstance.Spends.ListSpendByPeriod(username, accountPeriodIds);
		//    return spends;
		//}

		//[HttpPost]
		//public void AddSpend(AddSpendModel model)
		//{
		//    _backendInstance.Spends.AddSpend(model.Username, model.SpendType, model.Date, model.Amount, model.AccountPeriodIds);
		//}

		//[HttpPost]
		//public void AddSpendByAccount(AddSpendByAccountModel model)
		//{
		//    _backendInstance.Spends.AddSpendByAccount(model.Username, model.SpendType, model.Date, model.Amount,
		//                                              model.AccountIds);
		//}



		#endregion
	}
}