using System;
using System.Configuration;
using MyFinanceModel;

namespace MyFinanceWebApi.Models
{
	[Serializable]
    internal class ConfigurationMissingException : ConfigurationErrorsException
    {
        public ConfigurationMissingException(string settingName)
            : base(GetMessage(settingName))
        {
        }

        private static string GetMessage(string settingName)
        {
            return $"Setting {settingName} has not been set";
        }
    }

    [Serializable]
    internal class ConfigurationInvalidException : ConfigurationErrorsException
    {
        public ConfigurationInvalidException(string settingName)
            : base(GetMessage(settingName))
        {

        }

        private static string GetMessage(string settingName)
        {
            return $"Setting {settingName} has an invalid value";
        }
    }

    [Serializable]
    internal class UnauthorizeAccessException : ServiceException
    {
        public UnauthorizeAccessException(string message = "User is not allowed to perform this action") : base(message,
            1, System.Net.HttpStatusCode.Unauthorized)
        {
        }
    }

	internal class RequiredHeaderException: ServiceException
	{
		public RequiredHeaderException(ServiceAppHeader header) :base($"Header {header.Name} is required for this request",1,System.Net.HttpStatusCode.BadRequest)
		{
			RequiredHeader = header;
		}

		public ServiceAppHeader RequiredHeader { get; private set; }
	}
        

}