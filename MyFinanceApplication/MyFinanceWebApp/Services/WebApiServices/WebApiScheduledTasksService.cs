using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel.ClientViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public interface IScheduledTasksService
	{
		Task CreateBasicAsync(ClientScheduledTask.Basic model, string token);
		Task CreateTransferAsync(ClientScheduledTask.Transfer model, string token);
	}

	public class WebApiScheduledTasksService : MvcWebApiBaseService, IScheduledTasksService
	{
		protected override string ControllerName => "scheduledTasks";

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