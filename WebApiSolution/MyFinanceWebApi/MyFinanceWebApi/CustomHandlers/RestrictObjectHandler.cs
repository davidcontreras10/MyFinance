using MyFinanceWebApi.QueryHeaders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MyFinanceWebApi.CustomHandlers
{
    public class RestrictObjectHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
            task =>
                 {
                     if (!HasRestrictAttribute(task.Result?.RequestMessage))
                     {
                         return task.Result;
                     }

                     if (!ResponseIsValid(task.Result))
                     {
                         return task.Result;
                     }

                     var restrictValue = GetRestrictQuery(task.Result.RequestMessage);
                     if (string.IsNullOrEmpty(restrictValue))
                     {
                         return task.Result;
                     }

                     var content = task.Result.Content as ObjectContent;
                     if(content != null && content.Value != null)
                     {
                         var result = QueryProcessor.ProcessQuery(content.Value, restrictValue, QueryProcessor.QueryActionTtype.Remove);
                         if(result != null)
                         {
                             var formatter = GlobalConfiguration.Configuration.Formatters.First(t => t.SupportedMediaTypes.Contains(new MediaTypeHeaderValue(task.Result.Content.Headers.ContentType.MediaType)));
                             task.Result.Content = new ObjectContent<JToken>(result, formatter);
                         }
                     } 

                     return task.Result;
                 }
             );
        }

        private bool ResponseIsValid(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK || !(response.Content is ObjectContent)) return false;
            return true;
        }

        private static bool HasRestrictAttribute(HttpRequestMessage requestMessage)
        {
            var actionDescriptor = requestMessage?.Properties.FirstOrDefault(p => p.Key == "MS_HttpActionDescriptor").Value as ReflectedHttpActionDescriptor;
            if(actionDescriptor == null)
            {
                return false;
            }

            return HasRestrictAttribute(actionDescriptor);
        }

        private static bool HasRestrictAttribute(ReflectedHttpActionDescriptor description)
        {
            var attributes = description.GetCustomAttributes<IncludeRestrictObjectHeaderAttribute>();
            return attributes.Any();
        }

        private static string GetRestrictQuery(HttpRequestMessage httpRequestMessage)
        {
            if(httpRequestMessage == null)
            {
                return null;
            }

            IEnumerable<string> restrictValues = new List<string>();
            var result = httpRequestMessage.Headers.TryGetValues("$restrict", out restrictValues);
            if(restrictValues!=null && restrictValues.Any())
            {
                return restrictValues.First();
            }

            return null;
        }
    }
}
