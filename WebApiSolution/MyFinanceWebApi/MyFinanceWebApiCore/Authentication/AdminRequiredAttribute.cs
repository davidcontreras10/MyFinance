using Microsoft.AspNetCore.Mvc.Filters;
using MyFinanceWebApiCore.Models;

namespace MyFinanceWebApiCore.Authentication
{
	public class AdminRequiredAttribute : IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext context)
		{
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.HttpContext.User.IsInRole("Admin"))
			{
				throw new UnauthorizeAccessException("Admin user required");
			}
		}
	}
}
