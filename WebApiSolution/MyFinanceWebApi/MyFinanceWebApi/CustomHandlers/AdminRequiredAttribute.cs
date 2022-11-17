using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MyFinanceWebApi.Models;

namespace MyFinanceWebApi.CustomHandlers
{
	public class AdminRequiredAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (!actionContext.RequestContext.Principal.IsInRole("Admin"))
			{
				throw new UnauthorizeAccessException("Admin user required");
			}
		}
	}
}