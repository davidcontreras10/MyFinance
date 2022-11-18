using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Security.OAuth;
using Autofac.Integration.Owin;
using Microsoft.Owin;
using MyFinanceBackend.Services;

namespace MyFinanceWebApi.Authorization
{
    public class SimpleAuthorizationServerProvider: OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
	        await Task.Run(() => { context.Validated(); });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
	        await Task.Run(() =>
	        {
		        if (context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
		        {
			        context.OwinContext.Response.Headers["Access-Control-Allow-Origin"] = "*";

		        }
		        else
		        {
					context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
				}

		        var userService = GetUserService(context.OwinContext);
		        var username = context.UserName;
		        var password = context.Password;
		        var loginResult = userService.AttemptToLogin(username, password);
		        if (!loginResult.IsAuthenticated)
		        {
			        context.SetError("invalid_grant", "The user name or password is incorrect.");
			        return;
		        }

		        var role = loginResult.User.Username == "AzureAdmin" ? "Admin" : string.Empty;
		        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
		        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
		        identity.AddClaim(new Claim(ClaimTypes.Role, role));
		        identity.AddClaim(new Claim(ClaimTypes.UserData, loginResult.User.UserId.ToString()));
		        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginResult.User.UserId.ToString()));

		        context.Validated(identity);
			});
        }

        private static IUsersService GetUserService(IOwinContext context)
        {
            var autofac = context.GetAutofacLifetimeScope();
            var userService = autofac.Resolve(typeof(IUsersService));
            if (!(userService is IUsersService))
            {
                throw new Exception("User service not found");
            }

            return (IUsersService)userService;
        }
    }
}