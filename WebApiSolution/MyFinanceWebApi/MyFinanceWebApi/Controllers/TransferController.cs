using System.Collections.Generic;
using System.Web.Http;
using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApi.Controllers
{
    public class TransferController : BaseController
    {
        #region Attributes

        private readonly ITransferService _transferService;

        private const string ROOT_ROUTE = "api/transfer/";

        #endregion

        #region Constructors

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        #endregion

        #region Action Methods

        [Route(ROOT_ROUTE + "possibleCurrencies")]
        [HttpGet]
        public IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId)
        {
            var userId = GetUserId();
            return _transferService.GetPossibleCurrencies(accountId, userId);
        }

        [Route(ROOT_ROUTE+"possibleDestination")]
        [HttpGet]
        public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId, BalanceTypes balanceType)
        {
            var userId = GetUserId();
            return _transferService.GetPossibleDestinationAccount(accountPeriodId, currencyId, userId,
                balanceType);
        }

        [Route(ROOT_ROUTE + "basicAccountInfo")]
        [HttpGet]
        public TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId)
        {
            var userId = GetUserId();
            return _transferService.GetBasicAccountInfo(accountPeriodId, userId);
        }

        [Route(ROOT_ROUTE)]
        [HttpPost]
        public IEnumerable<ItemModified> CreateTransfer(TransferClientViewModel transferClientViewModel)
        {
	        transferClientViewModel.UserId = GetUserId();
            return _transferService.SubmitTransfer(transferClientViewModel);
        }

        #endregion
    }
}
