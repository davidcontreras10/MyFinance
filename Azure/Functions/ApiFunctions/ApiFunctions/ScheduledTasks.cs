using System;
using System.Threading.Tasks;
using ApiFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ApiFunctions
{
	public class ScheduledTasks
	{
		// ReSharper disable UnusedMember.Local
		private const string TestCron = "0 */2 * * * *";
		private const string DailyCron = "0 0 3 * * *";
		private const string WeeklyCron = "0 0 3 * * 1";
		// ReSharper restore UnusedMember.Local

		private readonly IScheduledTasksService _scheduledTasksService;

		public ScheduledTasks(IScheduledTasksService scheduledTasksService)
		{
			_scheduledTasksService = scheduledTasksService;
		}

		[FunctionName(nameof(ScheduledTasks))]
		public async Task Run([TimerTrigger(TestCron)] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation($"C# ScheduledTasks executing at: {DateTime.Now}");
			await _scheduledTasksService.ExecuteAllTasksAsync();
		}
	}
}
