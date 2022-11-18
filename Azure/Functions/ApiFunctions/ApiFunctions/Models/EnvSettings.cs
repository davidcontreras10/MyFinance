using System;

namespace ApiFunctions.Models
{
	public interface IEnvSettings
	{
		string BaseAPIUrl { get; }
		string AzureAppUser { get; }
		string AzureAppPw { get; }
	}

	public class EnvSettings : IEnvSettings
	{
		public string BaseAPIUrl => Environment.GetEnvironmentVariable("webApiBaseApi");
		public string AzureAppUser => Environment.GetEnvironmentVariable("azureAppUser");
		public string AzureAppPw => Environment.GetEnvironmentVariable("azureAppPw");
	}
}
