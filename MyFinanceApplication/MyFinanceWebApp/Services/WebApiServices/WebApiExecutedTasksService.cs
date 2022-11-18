using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel.ViewModel;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
	public interface IExecutedTasksService
	{
		Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTaskAsync(string taskId, string token);
	}

	public class WebApiExecutedTasksService : MvcWebApiBaseService, IExecutedTasksService
	{
		protected override string ControllerName => "executedTasks";

		public async Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTaskAsync(string taskId, string token)
		{
			var parameters = new Dictionary<string, object>
			{
				{"taskId", taskId}
			};

			var url = CreateRootUrl(parameters);
			var request = new WebApiRequest(url, HttpMethod.Get, token);
			return await GetResponseAsAsync<IReadOnlyCollection<ExecutedTaskViewModel>>(request);
		}
	}
}