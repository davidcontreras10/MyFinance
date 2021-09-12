using System;
using System.Configuration;

namespace MyFinanceWebApp.Models
{
    internal class UserAuthenticationException : Exception
    {
        public UserAuthenticationException()
            : base(DEFAULT_MESSAGE)
        {
            ExceptionInfo = null;
        }

        public UserAuthenticationException(object exceptionInfo)
            : base(DEFAULT_MESSAGE)
        {
            ExceptionInfo = exceptionInfo;
        }

        public object ExceptionInfo { get; }

        private const string DEFAULT_MESSAGE = "Fail to autenticate user";
    }

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
    internal class ObjectTypeNotSupportedException : Exception
    {
        public ObjectTypeNotSupportedException(Type type) : base($"Type {type.FullName} is not supported for this operation.") { }
    }

    [Serializable]
    internal class InvalidObjectTypeException : Exception
    {
        public InvalidObjectTypeException(Type type) : base($"Type {type.FullName} is invalid for this operation.") { }
    }
}