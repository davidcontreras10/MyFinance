using ApiFunctions.Models;
using ApiFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ApiFunctions.Startup))]
namespace ApiFunctions
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddHttpClient();
			builder.Services.AddLogging();
			Register(builder.Services);
		}

		private static void Register(IServiceCollection services)
		{
			services.AddScoped<IScheduledTasksService, ScheduledTasksService>();
			services.AddSingleton<IEnvSettings, EnvSettings>();
		}
	}
}
