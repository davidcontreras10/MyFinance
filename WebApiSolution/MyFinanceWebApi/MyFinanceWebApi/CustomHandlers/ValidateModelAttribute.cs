using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MyFinanceModel;

namespace MyFinanceWebApi.CustomHandlers
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public int ErrorCode { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }


        public ValidateModelStateAttribute(string message = "ModelState error", int errorCode = 5000,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            ErrorCode = errorCode;
            Message = message;
            StatusCode = statusCode;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (SkipValidation(actionContext) || actionContext.ModelState.IsValid)
                return;
            var exception = new ModelStateException(actionContext.ModelState, Message, ErrorCode, StatusCode);
            throw exception;
        }

        private static bool SkipValidation(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AvoidModelStateValidationAttribute>().Any() || actionContext
                       .ControllerContext.ControllerDescriptor.GetCustomAttributes<AvoidModelStateValidationAttribute>()
                       .Any();
        }
    }
}