using System;
using System.Configuration;
using MyFinanceWebApi.Models;

namespace MyFinanceWebApi.Helpers
{
    internal static class CustomAppSettings
    {
        private static TimeSpan? _authExpireToken;
        public static TimeSpan AuthExpireToken
        {
            get
            {
                if (_authExpireToken != null)
                {
                    return (TimeSpan)_authExpireToken;
                }

                var timeType = ConfigurationManager.AppSettings["expire_token_value_type"];
                if (string.IsNullOrEmpty(timeType))
                {
                    throw new ConfigurationMissingException("expire_token_value_type");
                }

                var timeoutSettingValue = ConfigurationManager.AppSettings["expire_token_value"];
                if (string.IsNullOrEmpty(timeType))
                {
                    throw new ConfigurationMissingException("expire_token_value");
                }

                double timeoutValue;
                if (!double.TryParse(timeoutSettingValue, out timeoutValue))
                {
                    throw new ConfigurationInvalidException("expire_token_value");
                }

                TimeSpan timeout;
                switch (timeType)
                {
                    case "ss":
                        timeout = TimeSpan.FromSeconds(timeoutValue);
                        break;
                    case "mm":
                        timeout = TimeSpan.FromMinutes(timeoutValue);
                        break;
                    case "dd":
                        timeout = TimeSpan.FromDays(timeoutValue);
                        break;
                    case "hh":
                        timeout = TimeSpan.FromHours(timeoutValue);
                        break;
                    default:
                        throw new ConfigurationInvalidException("expire_token_value_type");
                }

                _authExpireToken = timeout;
                return _authExpireToken.Value;
            }
        }
    }
}