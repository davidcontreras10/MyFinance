using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceModel;
using System.Collections.Generic;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransfersController : BaseApiController
	{
		#region Attributes

		private readonly ITransferService _transferService;

		#endregion

		#region Constructors

		public TransfersController(ITransferService transferService)
		{
			_transferService = transferService;
		}

		#endregion

		#region Action Methods

		[Route("possibleCurrencies")]
		[HttpGet]
		public IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId)
		{
			var userId = GetUserId();
			return _transferService.GetPossibleCurrencies(accountId, userId);
		}

		[Route("possibleDestination")]
		[HttpGet]
		public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId, BalanceTypes balanceType)
		{
			var userId = GetUserId();
			return _transferService.GetPossibleDestinationAccount(accountPeriodId, currencyId, userId,
				balanceType);
		}

		[Route("basicAccountInfo")]
		[HttpGet]
		public TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId)
		{
			var userId = GetUserId();
			return _transferService.GetBasicAccountInfo(accountPeriodId, userId);
		}

		[HttpPost]
		public IEnumerable<ItemModified> CreateTransfer(TransferClientViewModel transferClientViewModel)
		{
			transferClientViewModel.UserId = GetUserId();
			return _transferService.SubmitTransfer(transferClientViewModel);
		}

		#endregion
	}
}
