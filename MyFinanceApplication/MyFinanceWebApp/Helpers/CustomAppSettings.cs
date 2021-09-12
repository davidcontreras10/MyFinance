using System;
using System.Configuration;
using MyFinanceWebApp.Models;

namespace MyFinanceWebApp.Helpers
{
	internal static class CustomAppSettings
	{
		private static TimeSpan? _refreshAuthTokenTime;
		public static TimeSpan RefreshAuthTokenTime
		{
			get
			{
				if (_refreshAuthTokenTime != null)
				{
					return (TimeSpan)_refreshAuthTokenTime;
				}

				var timeType = ConfigurationManager.AppSettings["refresh_token_value_type"];
				if (string.IsNullOrEmpty(timeType))
				{
					throw new ConfigurationMissingException("refresh_token_value_type");
				}

				var timeoutSettingValue = ConfigurationManager.AppSettings["refresh_token_value"];
                if (string.IsNullOrEmpty(timeoutSettingValue))
				{
					throw new ConfigurationMissingException("refresh_token_value");
				}

				double timeoutValue;
				if (!double.TryParse(timeoutSettingValue, out timeoutValue))
				{
					throw new ConfigurationInvalidException("refresh_token_value");
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
						throw new ConfigurationInvalidException("refresh_token_value_type");
				}

				_refreshAuthTokenTime = timeout;
				return (TimeSpan)_refreshAuthTokenTime;
			}
		}
	}
}