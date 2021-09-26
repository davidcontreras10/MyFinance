using System;
using MyFinance.MyFinanceWebApp.Models;
using MyFinance.WebApiBaseConsumer;

namespace MyFinance.MyFinanceWebApp.Services.WebApiServices
{
    public abstract class MvcWebApiBaseService : WebApiBaseService
    {
	    private readonly IAppSettings _appSettings;

	    protected MvcWebApiBaseService(IAppSettings appSettings)
	    {
		    _appSettings = appSettings;
	    }

	    protected override string GetApiBaseDomain()
	    {
		    var domainValue = _appSettings.MyFinanceWsServer;
			if (string.IsNullOrEmpty(domainValue))
				throw new Exception("MyFinanceWsServer value is not set");
			return domainValue;
		}
	}
}


