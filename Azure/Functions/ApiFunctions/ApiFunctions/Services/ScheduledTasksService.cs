﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ApiFunctions.Models;
using System.Net.Http.Headers;
using System;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Linq;

namespace ApiFunctions.Services
{
	public interface IScheduledTasksService
	{
		Task ExecuteAllTasksAsync();
	}

	public class ScheduledTasksService : IScheduledTasksService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IEnvSettings _envSettings;
		private readonly ILogger<IScheduledTasksService> _logger;

		public ScheduledTasksService(IHttpClientFactory httpClientFactory, IEnvSettings envSettings, ILogger<IScheduledTasksService> logger)
		{
			_httpClientFactory = httpClientFactory;
			_envSettings = envSettings;
			_logger = logger;
		}

		public async Task ExecuteAllTasksAsync()
		{
			var apiCredentials = new ApiCredentials
			{
				Password = _envSettings.AzureAppPw,
				Username = _envSettings.AzureAppUser
			};

			var tokenResponse = await GetTokenAsync(apiCredentials);
			var tasks = await GetTodaysScheduledTasksAsync(tokenResponse.AccessToken);
			var taskIds = tasks.Select(t => t.Id.ToString()).ToList();
			await ExecuteTasksAsync(taskIds, tokenResponse.AccessToken);
		}

		private async Task ExecuteTasksAsync(IReadOnlyCollection<string> taskIds, string token)
		{
			foreach(var taskId in taskIds)
			{
				var result = await ExecuteTaskAsync(taskId, token);
				if(result.Status == ExecutedTaskStatus.Failed)
				{
					_logger.LogWarning($"TaskId: {result.TaskId} failed. Error: {result.ErrorMsg}");
				}
				else
				{
					_logger.LogInformation($"TaskId: {result.TaskId} was successful");
				}
			}			
		}

		private async Task<TaskExecutedResult> ExecuteTaskAsync(string taskId, string token)
		{
			var url = $"{_envSettings.BaseAPIUrl}api/scheduledTasks/{taskId}/execution";
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			return await CallServiceAsync<TaskExecutedResult>(request, new ClientExecuteTask
			{
				DateTime = DateTime.UtcNow,
				RequestType = ExecuteTaskRequestType.Automatic
			});
		}

		private async Task<IReadOnlyCollection<SimpleScheduledTask>> GetTodaysScheduledTasksAsync(string token)
		{
			var url = $"{_envSettings.BaseAPIUrl}api/scheduledTasks";
			var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			return await CallServiceAsync<IReadOnlyCollection<SimpleScheduledTask>>(request);
		}

		private async Task<AuthTokenResponse> GetTokenAsync(ApiCredentials apiCredentials)
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
			return await CallServiceAsync<AuthTokenResponse>(request);
		}

		private async Task<T> CallServiceAsync<T>(HttpRequestMessage httpRequest, object jsonModel = null)
		{
			try
			{
				if (jsonModel != null)
				{
					httpRequest.Content = new StringContent(JsonConvert.SerializeObject(jsonModel), Encoding.UTF8, Application.Json);
				}

				var client = _httpClientFactory.CreateClient();
				var response = await client.SendAsync(httpRequest);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsAsync<T>();
				}

				var stringRes = await response.Content.ReadAsStringAsync();
				_logger.LogError(stringRes);
				throw new Exception(stringRes);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
				throw;
			}

		}
	}
}
