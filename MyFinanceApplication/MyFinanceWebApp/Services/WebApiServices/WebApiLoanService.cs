using System;
using System.Collections.Generic;
using System.Net.Http;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
    public class WebApiLoanService : MvcWebApiBaseService, ILoanService
    {
        protected override string ControllerName => CoreVersion ? "loans" : "loan";

        public IEnumerable<SpendItemModified> AddLoanSpend(ClientLoanSpendViewModel clientLoanSpendViewModel, string token)
        {
            var url = CreateMethodUrl("payment");
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = clientLoanSpendViewModel
            };

            var response = GetResponseAs<IEnumerable<SpendItemModified>>(request);
            return response;
        }

        public IEnumerable<SpendItemModified> CreateLoan(ClientLoanViewModel clientLoanViewModel, string token)
        {
            var url = CreateRootUrl();
            var request = new WebApiRequest(url, HttpMethod.Post, token)
            {
                Model = clientLoanViewModel
            };

            var response = GetResponseAs<IEnumerable<SpendItemModified>>(request);
            return response;
        }

        public AddLoanRecordViewModel GetAddLoanRecordViewModel(int accountId, DateTime dateTime, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(accountId), accountId},
                {nameof(dateTime), dateTime }
            };

            var method = "add";
            var url = CreateMethodUrl(method, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<AddLoanRecordViewModel>(request);
            return response;
        }

        public IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByCriteriaId(string token, int loanRecordStatusId,
            LoanQueryCriteria criteriaId = LoanQueryCriteria.Invalid, IEnumerable<int> ids = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(criteriaId), (int)criteriaId},
                {nameof(loanRecordStatusId), loanRecordStatusId}
            };

            if (ids != null)
            {
                parameters.Add("ids", ids);
            }

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<IEnumerable<LoanReportViewModel>>(request);
            return response;
        }

        public LoanReportViewModel GetLoanDetailRecordsById(int loanRecordId, bool lowConsume, string token)
        {
            var url = CreateCustomUrl(loanRecordId.ToString());

            var request = new WebApiRequest(url, HttpMethod.Get, token);
            if (lowConsume)
            {
                var headers = new Dictionary<string, string>
                {
                    { "$restrict", "SpendViewModels" }
                };

                request.Headers = headers;
            }
                
            var response = GetResponseAs<LoanReportViewModel>(request);
            return response;
        }

        public AddLoanSpendViewModel GetAddLoanSpendViewModel(int loanRecordId, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(loanRecordId), loanRecordId}
            };

            var method = "add/payment";
            var url = CreateMethodUrl(method, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<AddLoanSpendViewModel>(request);
            return response;
        }

        public IEnumerable<AccountDetailsViewModel> GetSupportedLoanAccount(string token)
        {
            const string method = "accounts";
            var url = CreateMethodUrl(method);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<IEnumerable<AccountDetailsViewModel>>(request);
            return response;
        }

        public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountId, DateTime dateTime,
            int currencyId, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(accountId), accountId},
                {nameof(dateTime), dateTime },
                {nameof(currencyId), currencyId}
            };

            const string method = "destinationAccounts";
            var url = CreateMethodUrl(method, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var response = GetResponseAs<IEnumerable<AccountViewModel>>(request);
            return response;
        }

        public IEnumerable<ItemModified> DeleteLoan(int loanRecordId, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(loanRecordId), loanRecordId}
            };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Delete, token);
            var response = GetResponseAs<IEnumerable<ItemModified>>(request);
            return response;
        }

        public WebApiLoanService(IHttpClientFactory httpClientFactory) : base(httpClientFactory, true)
        {
        }
    }
}