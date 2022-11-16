using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ApiFunctions
{
    public class Function1
    {
		// ReSharper disable UnusedMember.Local
		private const string TestCron = "0 */2 * * * *";
		private const string DailyCron = "0 0 3 * * *";
		private const string WeeklyCron = "0 0 3 * * 1";
        // ReSharper restore UnusedMember.Local


		[FunctionName("Function1")]
        public void Run([TimerTrigger(TestCron)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Custom Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
