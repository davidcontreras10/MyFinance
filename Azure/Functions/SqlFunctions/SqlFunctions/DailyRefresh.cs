using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SqlFunctions
{
    public static class DailyRefresh
    {
	    
	    // ReSharper disable UnusedMember.Local
	    private const string TestCron = "0 */2 * * * *";
	    private const string DailyCron = "0 0 3 * * *";
	    private const string WeeklyCron = "0 0 3 * * 1";
	    // ReSharper restore UnusedMember.Local

		[FunctionName(nameof(DailyRefresh))]
        public static async Task Run([TimerTrigger(DailyCron)]TimerInfo myTimer, ILogger log)
        {
	        var str = Environment.GetEnvironmentVariable("DefaultMYFNDB");
	        if (string.IsNullOrWhiteSpace(str))
	        {
		        const string error = "Connection string DefaultMYFNDB not found";
				log.LogError(error);
				throw new Exception(error);
	        }

			log.LogInformation("Connected. Executing script.");
			await using var conn = new SqlConnection(str);
			conn.Open();
			var text = @"
					BEGIN
						DECLARE @currentUserId UNIQUEIDENTIFIER
						DECLARE @currentDate DATETIME = GETDATE()
						DECLARE tempusersCursor CURSOR FAST_FORWARD LOCAL
						FOR SELECT UserId FROM dbo.AppUser
						OPEN tempusersCursor
						FETCH NEXT FROM tempusersCursor INTO @currentUserId
						WHILE @@FETCH_STATUS = 0
						BEGIN
							EXEC dbo.SpUpdateCurrentPeriods @pUserId = @currentUserId, @pDate = @currentDate
							FETCH NEXT FROM tempusersCursor INTO @currentUserId
						END
						CLOSE tempusersCursor
						INSERT INTO [dbo].[DailyJob] (JobDate, EventDesc) VALUES (@currentDate, 'Function local script EveryDay local');
					END
					";

			await using var cmd = new SqlCommand(text, conn) {CommandTimeout = conn.ConnectionTimeout};
			await cmd.ExecuteNonQueryAsync();
        }
    }
}
