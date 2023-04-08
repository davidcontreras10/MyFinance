using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;

namespace MyFinanceWebApp.CustomHandlers
{
	public class TokenAuthorizeAttribute : AuthorizeAttribute
	{
		public IUserService UserService
		{
			get
			{
				var lifetimeScope = AutofacDependencyResolver.Current.RequestLifetimeScope;
				var userService = lifetimeScope.Resolve(typeof(IUserService)) as IUserService;
				return userService;
			}
		}

        #region Implementation

        protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var authorizationTokenResult = AuthorizationTokenValidation(httpContext);
            return base.AuthorizeCore(httpContext) && authorizationTokenResult;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsAjaxRequest())
			{
				throw new BackendTokenException();
			}

			base.HandleUnauthorizedRequest(filterContext);
		}

        #endregion

        #region Privates

        private bool AuthorizationTokenValidation(HttpContextBase httpContext)
		{
            var result = RefreshToken(httpContext);
			return result;
        }

	    private bool RefreshToken(HttpContextBase httpContext)
	    {
            if (!httpContext.User.Identity.IsAuthenticated)
	        {
	            return false;
	        }

            var userId = httpContext.User.Identity.Name;
            var cookie = httpContext.Request.Cookies[Constants.AuthorizationCookieName];
			var tokenCookie = cookie.Values[Constants.AuthTokenCookieName];
			return !string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(tokenCookie);
	    }

        #endregion
    }
}