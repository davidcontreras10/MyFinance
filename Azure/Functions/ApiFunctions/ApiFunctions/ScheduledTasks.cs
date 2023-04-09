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
		private const string DailyCron = "0 10 6 * * *";
		private const string WeeklyCron = "0 0 3 * * 1";
		// ReSharper restore UnusedMember.Local

		private readonly IScheduledTasksService _scheduledTasksService;

		public ScheduledTasks(IScheduledTasksService scheduledTasksService)
		{
			_scheduledTasksService = scheduledTasksService;
		}

		[FunctionName(nameof(ScheduledTasks))]
		public async Task Run([TimerTrigger(DailyCron)] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation($"C# ScheduledTasks executing at: {DateTime.Now}");
			try
			{
				await _scheduledTasksService.ExecuteAllTasksAsync();
			}
			catch (Exception ex)
			{
				log.LogError(ex, "LogError ScheduledTasks");
			}
			log.LogInformation($"C# ScheduledTasks executed at: {DateTime.Now}");
		}
	}
}
