using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ApiFunctions.Models;

namespace ApiFunctions.Services
{
	public interface IScheduledTasksService
	{
		Task<AuthTokenResponse> ExecuteAllTasksAsync();
	}

	public class ScheduledTasksService : IScheduledTasksService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IEnvSettings _envSettings;
		private readonly ILogger _logger;

		public ScheduledTasksService(IHttpClientFactory httpClientFactory, IEnvSettings envSettings)
		{
			_httpClientFactory = httpClientFactory;
			_envSettings = envSettings;
			//_logger = logger;
		}

		public async Task<AuthTokenResponse> ExecuteAllTasksAsync()
		{
			var apiCredentials = new ApiCredentials
			{
				Password = _envSettings.AzureAppPw,
				Username = _envSettings.AzureAppUser
			};
			return await GetTokenAsync(apiCredentials);
			//_logger.LogInformation("Token response:", token);
		}

		private async Task<AuthTokenResponse> GetTokenAsync(ApiCredentials apiCredentials)
		{
			try
			{
				var tokenUrl = _envSettings.BaseAPIUrl + "token";
				var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
				var parameters = new Dictionary<string, string>
			{
				{"grant_type", "password"},
				{"username", apiCredentials.Username},
				{"password", apiCredentials.Password}
			};

				var content = new FormUrlEncodedContent(parameters);
				request.Content = content;
				var client = _httpClientFactory.CreateClient();
				var response = await client.SendAsync(request);
				if (response.IsSuccessStatusCode)
				{
					//_logger.LogInformation("Success token  retreival");
					return await response.Content.ReadAsAsync<AuthTokenResponse>();
				}

				var stringRes = await response.Content.ReadAsStringAsync();
				//_logger.LogError(stringRes);
			}
			catch (System.Exception ex)
			{
				//_logger.LogError(ex.ToString());
			}
			return null;
		}
	}
}
