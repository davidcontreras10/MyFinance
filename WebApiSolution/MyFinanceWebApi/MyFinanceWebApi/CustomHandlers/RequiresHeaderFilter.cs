using MyFinanceModel;
using MyFinanceWebApi.Models;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MyFinanceWebApi.CustomHandlers
{
	public class RequiresHeaderFilter : ActionFilterAttribute
	{
		public ServiceAppHeader RequiredServiceAppHeader { get; private set; }

		public RequiresHeaderFilter(ServiceAppHeader.ServiceAppHeaderType header)
		{
			RequiredServiceAppHeader = ServiceAppHeader.GetServiceAppHeader(header);
		}

		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (actionContext.Request.Headers.All(h => h.Key != RequiredServiceAppHeader.Name))
			{
				throw new RequiredHeaderException(RequiredServiceAppHeader);
			}

			base.OnActionExecuting(actionContext);
		}
	}
}