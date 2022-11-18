using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public interface IScheduledTasksService
	{
		Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledTasksAsync(string token);
		Task CreateBasicAsync(ClientScheduledTask.Basic model, string token);
		Task CreateTransferAsync(ClientScheduledTask.Transfer model, string token);
		Task DeleteTaskAsync(string taskId, string token);
		Task<TaskExecutedResult> ExecuteTaskAsync(string token, string taskId, DateTime dateTime);
	}

	public class WebApiScheduledTasksService : MvcWebApiBaseService, IScheduledTasksService
	{
		protected override string ControllerName => "scheduledTasks";

		public async Task DeleteTaskAsync(string taskId, string token)
		{
			var url = CreateMethodUrl(taskId);
			var request = new WebApiRequest(url, HttpMethod.Delete, token);
			await GetResponseAsync(request);
		}

		public async Task<TaskExecutedResult> ExecuteTaskAsync(string token, string taskId, DateTime dateTime)
		{
			var clientExecuteTask = new ClientExecuteTask
			{
				DateTime = dateTime,
				RequestType = ExecuteTaskRequestType.Manual
			};

			var url = CreateMethodUrl($"{taskId}/execution");
			var request = new WebApiRequest(url, HttpMethod.Post, token)
			{
				Model = clientExecuteTask
			};

			return await GetResponseAsAsync<TaskExecutedResult>(request);
		}

		public async Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledTasksAsync(string token)
		{
			var url = CreateMethodUrl("@current");
			var request = new WebApiRequest(url, HttpMethod.Get, token);
			return await GetResponseAsAsync<IReadOnlyCollection<BaseScheduledTaskVm>>(request);
		}

		public async Task CreateBasicAsync(ClientScheduledTask.Basic model, string token)
		{
			var url = CreateMethodUrl("basic");
			var request = new WebApiRequest(url, HttpMethod.Post, token)
			{
				Model = model,
				ProcessResponse = true
			};

			await GetResponseAsync(request);
		}

		public async Task CreateTransferAsync(ClientScheduledTask.Transfer model, string token)
		{
			var url = CreateMethodUrl("transfer");
			var request = new WebApiRequest(url, HttpMethod.Post, token)
			{
				Model = model,
				ProcessResponse = true
			};

			await GetResponseAsync(request);
		}
	}
}