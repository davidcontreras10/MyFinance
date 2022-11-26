using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MyFinanceWebApi.Authorization
{
	public class ExAuthorizeAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var anonymousRequest = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
			var bearerToken = actionContext.Request.Headers.Authorization?.Parameter ?? string.Empty;
			if (string.IsNullOrWhiteSpace(bearerToken))
			{
				if (anonymousRequest)
				{
					return;
				}

				throw new UnauthorizedAccessException("Access token is required");
			}

			bearerToken = bearerToken.Replace("bearer ", string.Empty);
			var secret = ConfigurationManager.AppSettings.Get("authSecret");
			var principal = GetToken(bearerToken, secret, anonymousRequest);
			if(principal == null)
			{
				throw new UnauthorizedAccessException();
			}

			actionContext.RequestContext.Principal = principal;
		}

		private ClaimsPrincipal GetToken(string token, string secret, bool anonymousRequest)
		{

			var tokenHandler = new JwtSecurityTokenHandler();
			if (string.IsNullOrEmpty(secret))
			{
				throw new Exception($"Expected: {nameof(secret)}");
			}

			if (!tokenHandler.CanReadToken(token))
			{
				if (anonymousRequest)
				{
					return null;
				}
				throw new UnauthorizedAccessException();
			}

			var key = Encoding.ASCII.GetBytes(secret);
			var tokenValidationParams = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			};

			try
			{
				SecurityToken securityToken;
				var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);
				return principal;

			}
			catch (Exception)
			{
				throw new UnauthorizedAccessException();
			}
		}
	}
}
