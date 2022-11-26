using MyFinanceWebApp.Models;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public interface IAuthenticationService
	{
		Task<AuthToken> GetAuthTokenAsync(string username, string password);
	}

	public class AuthenticationService : MvcWebApiBaseService, IAuthenticationService
	{
		public AuthenticationService(IHttpClientFactory httpClientFactory) : base(httpClientFactory, true)
		{
		}

		protected override string ControllerName => "Authentication";

		public async Task<AuthToken> GetAuthTokenAsync(string username, string password)
		{
			var authRequestModel = new AuthenticateRequest
			{
				Password = password,
				Username = username
			};

			var url = CreateRootUrl();
			var request = new WebApiRequest(url, HttpMethod.Post)
			{
				Model = authRequestModel
			};

			return await GetResponseAsAsync<AuthToken>(request);
		}
	}
}
