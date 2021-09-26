using System;
using Microsoft.Extensions.Configuration;
using MyFinance.Utilities;

namespace MyFinance.MyFinanceWebApp.Models
{
	public interface IAppSettings
	{
		string MyFinanceWsServer { get; }
		TimeSpan WebAuthExpireTime { get; }
	}

	public class AppSettings : IAppSettings
	{
		private readonly IConfiguration _configuration;

		private AppSettingsObject _appSettingsObject;

		private AppSettingsObject Get
		{
			get { return _appSettingsObject ??= ReadSettingsObject(); }
		}

		public AppSettings(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		public string MyFinanceWsServer => Get.MyFinanceUrls.MyFinanceApiServer;
		public TimeSpan WebAuthExpireTime => Get.WebAuth.CookieValidTime;

		private AppSettingsObject ReadSettingsObject()
		{
			return new AppSettingsObject
			{
				MyFinanceUrls = _configuration.GetSection("myFinanceUrls").Get<AppSettingsObject.MyFinanceUrlsSection>(),
				WebAuth = _configuration.GetSection("webAuth").Get<AppSettingsObject.WebAuthSection>(),
			};
		}

		public class AppSettingsObject
		{
			public MyFinanceUrlsSection MyFinanceUrls { get; set; }

			public WebAuthSection WebAuth { get; set; }	

			public class MyFinanceUrlsSection
			{
				public string MyFinanceApiServer { get; set; }
			}

			public class WebAuthSection
			{
				public TimeSpan CookieValidTime { get; set; }
			}
		}
	}
}
