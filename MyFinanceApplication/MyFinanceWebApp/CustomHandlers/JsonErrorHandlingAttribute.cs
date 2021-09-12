using System.Web.Mvc;
using MyFinanceModel;
using Newtonsoft.Json;

namespace MyFinanceWebApp.CustomHandlers
{
    public class JsonErrorHandlingAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var modelStateException = filterContext.Exception as ModelStateException;
            if (modelStateException != null)
            {
                var exception = modelStateException;
                filterContext.HttpContext.Response.StatusCode = (int)exception.StatusCode;
                filterContext.Result = CreateModelErrorActionResult(exception);
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        private static ActionResult CreateModelErrorActionResult(ModelStateException exception)
        {
            var jsonResult = new
            {
                errorMessage = exception.Message,
                errorCode = exception.ErrorCode,
                statusCode = exception.StatusCode,
                modelState = exception.ModelStateErrorsObject
            };
	        var json = JsonConvert.SerializeObject(jsonResult);
            return new ContentResult
            {
                Content = json
            };
        }
    }
}