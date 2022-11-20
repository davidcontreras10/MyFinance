using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Contants;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
    public class WebApiTransferService : MvcWebApiBaseService, ITransferService
    {
        public IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string token)
        {
            if (string.IsNullOrEmpty(token) || accountId == 0)
            {
                throw new Exception("Invalid parameters");
            }

            var parameters = new Dictionary<string, object>
                {
                    {"accountId", accountId}
                };

            var url = CreateMethodUrl(WebServicesConstants.GET_TRANSFER_POSSIBLE_CURRENCIES, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            return GetResponseAs<IEnumerable<CurrencyViewModel>>(request);
        }

        public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId, string token,
            BalanceTypes balanceType)
        {
            if (string.IsNullOrEmpty(token) || accountPeriodId == 0)
            {
                throw new Exception("Invalid parameters");
            }

            var parameters = new Dictionary<string, object>
            {
                {"accountPeriodId", accountPeriodId},
                {"currencyId", currencyId},
                {"balanceType", balanceType}
            };

            var url = CreateMethodUrl(WebServicesConstants.GET_TRANSFER_POSSIBLE_DESTINATION_ACCOUNTS, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            return GetResponseAs<IEnumerable<AccountViewModel>>(request);
        }

        public async Task<IEnumerable<AccountViewModel>> GetPossibleDestinationAccountAsync(
	        int accountPeriodId,
	        int currencyId,
	        string token,
	        BalanceTypes balanceType
        )
        {
	        if (string.IsNullOrEmpty(token) || accountPeriodId == 0)
	        {
		        throw new Exception("Invalid parameters");
	        }

	        var parameters = new Dictionary<string, object>
	        {
		        {"accountPeriodId", accountPeriodId},
		        {"currencyId", currencyId},
		        {"balanceType", balanceType}
	        };

	        var url = CreateMethodUrl(WebServicesConstants.GET_TRANSFER_POSSIBLE_DESTINATION_ACCOUNTS, parameters);
	        var request = new WebApiRequest(url, HttpMethod.Get, token);
	        return await GetResponseAsAsync<IEnumerable<AccountViewModel>>(request);
        }

        public TransferAccountDataViewModel GetBasicAccountInfo(int accountPeriodId, string token)
        {
            if (string.IsNullOrEmpty(token) || accountPeriodId == 0)
            {
                throw new Exception("Invalid parameters");
            }

            var parameters = new Dictionary<string, object>
                {
                    {"accountPeriodId", accountPeriodId}
                };

            var url = CreateMethodUrl(WebServicesConstants.GET_TRANSFER_ACCOUNT_BASIC_INFO, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            return GetResponseAs<TransferAccountDataViewModel>(request);
        }

        public IEnumerable<ItemModified> SubmitTransfer(string token, TransferClientViewModel transferClientViewModel)
        {
            if (transferClientViewModel == null)
            {
                throw new ArgumentNullException(nameof(transferClientViewModel));
            }

	        var url = CreateMethodUrl("");
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = transferClientViewModel
            };

            return GetResponseAs<IEnumerable<ItemModified>>(request);
        }

		protected override string ControllerName => "Transfer";

		public WebApiTransferService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
		{
		}
    }
}