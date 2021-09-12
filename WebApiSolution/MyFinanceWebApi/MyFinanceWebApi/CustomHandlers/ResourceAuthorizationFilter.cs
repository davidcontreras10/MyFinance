using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.AspNet.Identity;
using MyFinanceBackend.Attributes;
using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceWebApi.Models;

namespace MyFinanceWebApi.CustomHandlers
{
    public class ResourceAuthorizationFilter : ActionFilterAttribute
    {
        #region Attributes

        private readonly string _targetIdParamName;
        private readonly bool _isTargetArray;

        #endregion

        public ResourceAuthorizationFilter(string targetIdParamName = "targetUserId", bool isArray = false)
        {
            _targetIdParamName = targetIdParamName;
            _isTargetArray = isArray;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var resourcesAttribute = CreateResourceActionRequiredAttribute(actionContext);
            if (resourcesAttribute == null)
            {
                return;
            }

            var targetIds = ReadTargetIds(actionContext);
            var authenticatedUserId = GetUserId(actionContext);
            var authorizationService = GetAuthorizationService(actionContext);
            var result = authorizationService.IsAuthorized(authenticatedUserId, targetIds, resourcesAttribute);
            if (!result)
            {
                throw new UnauthorizeAccessException();
            }
        }

        #region Privates

        private IAuthorizationService GetAuthorizationService(HttpActionContext actionContext)
        {
            return (IAuthorizationService) actionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                typeof(IAuthorizationService));
        }

        private string GetUserId(HttpActionContext actionContext)
        {
            var userId = actionContext.RequestContext.Principal.Identity.GetUserId();
            return userId;
        }

        private IEnumerable<string> ReadTargetIds(HttpActionContext actionContext)
        {
            var idsObject = actionContext.ActionArguments[_targetIdParamName];
            if (idsObject == null)
            {
                throw new ArgumentNullException(nameof(idsObject));
            }

            if (_isTargetArray)
            {
                return (string[]) idsObject;
            }

            return new[] {idsObject.ToString()};
        }

        private ResourceActionRequiredAttribute CreateResourceActionRequiredAttribute(
            HttpActionContext actionContext)
        {
            var dataAttributes = GetCustomAttributeData(actionContext);
            var resourceActionRequiredAttributes =
                dataAttributes.Select(CreateResourceActionRequiredAttribute).Where(a => a != null);
            return resourceActionRequiredAttributes.FirstOrDefault();
        }

        private IEnumerable<CustomAttributeData> GetCustomAttributeData(HttpActionContext actionContext)
        {
            return !(actionContext.ActionDescriptor is ReflectedHttpActionDescriptor)
                ? new List<CustomAttributeData>()
                : ((ReflectedHttpActionDescriptor) actionContext.ActionDescriptor).MethodInfo.CustomAttributes;
        }

        private static ResourceActionRequiredAttribute CreateResourceActionRequiredAttribute(
            CustomAttributeData customAttributeData)
        {
            if (customAttributeData == null ||
                customAttributeData.AttributeType != typeof(ResourceActionRequiredAttribute))
                return null;

            var resource = (ApplicationResources)
                customAttributeData.ConstructorArguments.First(a => a.ArgumentType == typeof(ApplicationResources))
                    .Value;
            var actionsArray = (ReadOnlyCollection<CustomAttributeTypedArgument>)customAttributeData.ConstructorArguments
                .First(a => a.ArgumentType != typeof(ApplicationResources))
                .Value;
            var actions = actionsArray.Select(a =>
                (ResourceActionNames) Enum.Parse(typeof(ResourceActionNames), a.Value.ToString())).ToArray();
            return new ResourceActionRequiredAttribute(resource, actions);
        }

        #endregion  
    }
}