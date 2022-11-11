using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MyFinanceBackend.Services;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApi.Controllers
{
	[RoutePrefix(RootRoute)]
    public class ExecutedTasksController : BaseController
    {
	    private const string RootRoute = "api/executedTasks";

	    private readonly IScheduledTasksService _scheduledTasksService;

	    public ExecutedTasksController(IScheduledTasksService scheduledTasksService)
	    {
		    _scheduledTasksService = scheduledTasksService;
	    }

		[HttpGet]
		[Route("")]
	    public async Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTasksByTaskIdAsync(
		    [FromUri] string taskId
	    )
	    {
		    return await _scheduledTasksService.GetExecutedTasksByTaskIdAsync(taskId);
	    }
    }
}
