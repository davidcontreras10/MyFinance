using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MyFinanceWebApp.Helpers;
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
            var cookie = httpContext.Request.Cookies["TokenAuthorization"];

	        var lastUpdateCookie = cookie?.Values["LastUpdate"];
	        if (string.IsNullOrEmpty(lastUpdateCookie))
	        {
	            return false;
	        }

            var latestUpdate = DateTime.ParseExact(lastUpdateCookie, "O", CultureInfo.InvariantCulture);
		    if (DateTime.Now < latestUpdate.Add(CustomAppSettings.RefreshAuthTokenTime))
		    {
			    return true;
		    }

		    var refreshTokenEncrypted = cookie.Values["RefreshToken"];
	        if (string.IsNullOrEmpty(refreshTokenEncrypted))
	        {
	            return false;
	        }

            var refreshToken = LocalHelper.UnProtect(refreshTokenEncrypted, userId);
	        var newToken = UserService.RefreshToken(refreshToken);
	        if (newToken == null)
	        {
	            return false;
	        }

            var encryptRefreshToken = LocalHelper.Protect(newToken.RefreshToken, userId);
            var encryptAccessToken = LocalHelper.Protect(newToken.AccessToken, userId);
            var lastUpdatedString = DateTime.Now.ToString("O");
            cookie.Values["RefreshToken"] = encryptRefreshToken;
	        cookie.Values["AuthToken"] = encryptAccessToken;
	        cookie.Values["LastUpdate"] = lastUpdatedString;
	        httpContext.Response.Cookies.Add(cookie);
	        return true;
	    }

        #endregion
    }
}