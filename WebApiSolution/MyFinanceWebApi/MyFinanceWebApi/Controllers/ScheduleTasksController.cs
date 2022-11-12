using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApi.Controllers
{
	[RoutePrefix(RootRoute)]
	public class ScheduleTasksController : BaseController
    {
	    private const string RootRoute = "api/scheduledTasks";

	    private readonly IScheduledTasksService _scheduledTasksService;

	    public ScheduleTasksController(IScheduledTasksService scheduledTasksService)
	    {
		    _scheduledTasksService = scheduledTasksService;
	    }

		[Route("basic")]
		[HttpPost]
	    public async Task CreateBasicAsync(ClientScheduledTask.Basic model)
	    {
		    var userId = GetUserId();
		    await _scheduledTasksService.CreateBasicTrxAsync(userId, model);
	    }

	    [Route("transfer")]
	    [HttpPost]
	    public async Task CreateTransferAsync(ClientScheduledTask.Transfer model)
	    {
		    var userId = GetUserId();
		    await _scheduledTasksService.CreateTransferTrxAsync(userId, model);
	    }

	    [Route("@current")]
	    [HttpGet]
	    public async Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledTaskAsync()
	    {
		    var userId = GetUserId();
		    return await _scheduledTasksService.GetScheduledTasksByUserIdAsync(userId);
	    }

		[HttpDelete]
		[Route("{taskId}")]
	    public async Task DeleteScheduledTaskAsync(string taskId)
	    {
		    await _scheduledTasksService.DeleteByIdAsync(taskId);
	    }
	}
}
