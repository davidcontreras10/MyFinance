using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using MyFinanceModel;
using System;
using Serilog;
using System.Web.Http.Controllers;

namespace MyFinanceWebApi.CustomHandlers
{
    public class ErrorHandlerExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var logger = GetLogger(actionExecutedContext.ActionContext);
            logger.Error(actionExecutedContext.Exception, actionExecutedContext.Request.ToString());
            if (actionExecutedContext.Exception is ModelStateException)
            {
                actionExecutedContext.Response = CreateResponse(actionExecutedContext,
                    (ModelStateException) actionExecutedContext.Exception);
            }

            else if (actionExecutedContext.Exception is ServiceException)
            {
                actionExecutedContext.Response = CreateResponse(actionExecutedContext,
                    (ServiceException) actionExecutedContext.Exception);
            }

            else
            {
                actionExecutedContext.Response = CreateResponse(actionExecutedContext,
                    new ServiceException());
            }
        }

        private ILogger GetLogger(HttpActionContext actionContext)
        {
            return (ILogger)actionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                typeof(ILogger));
        }

        private static HttpResponseMessage CreateResponse(HttpActionExecutedContext actionExecutedContext,
            ServiceException serviceException)
        {
            var error = new
            {
                serviceException.Message,
                serviceException.ErrorCode,
                actionExecutedContext.Exception,
                serviceException.StatusCode
            };

            var response =
                actionExecutedContext.ActionContext.Request.CreateResponse(
                    serviceException.StatusCode, error);
            var errorContent = response.Content as ObjectContent<HttpError>;
            if (errorContent != null)
            {
                ((HttpError)errorContent.Value)["Message"] = serviceException.Message;
                ((HttpError)errorContent.Value)["ErrorCode"] = serviceException.ErrorCode;
                ((HttpError)errorContent.Value)["StatusCode"] = serviceException.StatusCode;
            }

            AddResponseErrorContentCode(response, serviceException.ErrorCode);
            return response;
        }

        private static HttpResponseMessage CreateResponse(HttpActionExecutedContext actionExecutedContext,
            ModelStateException modelStateException)
        {
            if (modelStateException.ModelStateErrorsObject is ModelStateDictionary)
            {
                var response2 =
                    actionExecutedContext.ActionContext.Request.CreateErrorResponse(
                        modelStateException.StatusCode,
                        (ModelStateDictionary) modelStateException.ModelStateErrorsObject);
                var errorContent = response2.Content as ObjectContent<HttpError>;
                if (errorContent != null)
                {
                    ((HttpError) errorContent.Value)["Message"] = modelStateException.Message;
                    ((HttpError) errorContent.Value)["ErrorCode"] = modelStateException.ErrorCode;
                    ((HttpError)errorContent.Value)["StatusCode"] = modelStateException.StatusCode;
                }

                AddResponseErrorContentCode(response2, modelStateException.ErrorCode);
                return response2;
            }

            var error = new
            {
                modelStateException.StatusCode,
                modelStateException.Message,
                modelStateException.ErrorCode,
                actionExecutedContext.Exception,
                modelStateException.ModelStateErrorsObject
            };

            var response =
                actionExecutedContext.ActionContext.Request.CreateResponse(
                    modelStateException.StatusCode, error);
            AddResponseErrorContentCode(response, modelStateException.ErrorCode);
            return response;
        }

        private static void AddResponseErrorContentCode(HttpResponseMessage response, int code)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.Headers.Add("ErrorContentType", code.ToString());
        }
    }
}