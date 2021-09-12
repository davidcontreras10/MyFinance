using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using MyFinanceWebApp.Helpers;

namespace MyFinanceWebApp.CustomHandlers
{
    public class HandleTokenErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is BackendTokenException)
            {
                FormsAuthentication.SignOut();
                filterContext.HttpContext.Response.StatusCode = 400;
	            if (filterContext.HttpContext.Request.IsAjaxRequest())
	            {
					filterContext.ExceptionHandled = true;
		            filterContext.Result = CreateErrorActionResult(filterContext);
	            }
	            else
	            {
					filterContext.ExceptionHandled = true;
		            filterContext.Result = new HttpUnauthorizedResult();
	            }
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        private static ActionResult CreateErrorActionResult(ExceptionContext filterContext)
        {
            var jsonResult = new
                {
                    errorMessage = filterContext.Exception.Message,
                    errorCode = 2000,
                    actionUrl = LocalHelper.ResolveServerUrl(FormsAuthentication.LoginUrl, false)
                };
            var json = new JavaScriptSerializer().Serialize(jsonResult);
            return new ContentResult
                {
                    Content = json
                };
        }
    }
}