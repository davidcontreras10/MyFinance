using Microsoft.AspNet.Identity;
using MyFinanceModel;
using System;
using System.Linq;
using System.Web.Http;

namespace MyFinanceWebApi.Controllers
{
	public class BaseController : ApiController
	{
		#region Protected Methods

		protected string GetUserId()
		{
			var userId = RequestContext.Principal.Identity.GetUserId();
			return userId;
		}

		protected ApplicationModules GetModuleNameValue()
		{
			var header = ServiceAppHeader.GetServiceAppHeader(ServiceAppHeader.ServiceAppHeaderType.ApplicationModule);
			if (ControllerContext.Request.Headers.Contains(header.Name))
			{
				var headerValue = ControllerContext.Request.Headers.GetValues(header.Name).FirstOrDefault();
				var module = (ApplicationModules)Enum.Parse(typeof(ApplicationModules), headerValue);
				return module;
			}

			return ApplicationModules.Unknown;
		}

		#endregion
	}
}