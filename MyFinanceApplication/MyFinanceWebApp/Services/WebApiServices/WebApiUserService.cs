using System;
using MyFinanceModel;
using MyFinanceWebApp.Contants;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Models;
using WebApiBaseConsumer;

namespace MyFinanceWebApp.Services.WebApiServices
{
    public class WebApiUserService : MvcWebApiBaseService, IUserService
    {
	    public AuthToken GetAuthToken(string username, string password)
	    {
            if (CoreVersion)
            {
                return GetAuthTokenCore(username, password);
            }

		    var url = GetBaseUrl("token");
		    var parameters = new Dictionary<string, string>
		    {
			    {"grant_type", "password"},
			    {"username", username},
			    {"password", password}
		    };

		    var content = new FormUrlEncodedContent(parameters);
            var request = new WebApiRequest(url, HttpMethod.Post)
            {
                IsJsonModel = false,
                UseControllerBaseUrl = false,
                Model = content,
            };

            return GetResponseAs<AuthToken>(request);
	    }

        private AuthToken GetAuthTokenCore(string username, string password)
        {
            var url = GetBaseUrl("authentication");
            var authModel = new
            {
				username,
				password
			};

            var request = new WebApiRequest(url, HttpMethod.Post)
            {
                Model = authModel
            };
            return GetResponseAs<AuthToken>(request);
		}

		public async Task<LoginResult> AttemptLoginAsync(string username, string password)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"username", username},
                    {"password", password}
                };
            var url = CreateMethodUrl(WebServicesConstants.RESULT_LOGIN_METHOD, parameters);
            var request = new WebApiRequest(url, HttpMethod.Get);
            var result = await GetResponseAsAsync<LoginResult>(request);
            return result ?? new LoginResult();
        }

	    public bool IsSessionValid(string token)
	    {
			var url = CreateMethodUrl(WebServicesConstants.IN_SESSION);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var result = GetResponseAs<bool>(request);
		    return result;
	    }

        public AuthToken RefreshToken(string refreshToken)
        {
            var url = GetBaseUrl("token");
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken},
                {"client_id", ""}
            };

            var content = new FormUrlEncodedContent(parameters);
            var request = new WebApiRequest(url, HttpMethod.Post)
            {
                Model = content,
                UseControllerBaseUrl = false,
                IsJsonModel = false
            };

            return GetResponseAs<AuthToken>(request);
        }

        public ResetPasswordValidationResult ValidateResetPasswordActionResult(string actionLink)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(actionLink), actionLink}
            };

            var url = CreateMethodUrl("ResetPassword", parameters);
            var request = new WebApiRequest(url, HttpMethod.Get);
            var result = GetResponseAs<ResetPasswordValidationResult>(request);
            return result;
        }

        public TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest)
        {
            var url = CreateMethodUrl("ResetPassword");
            var request = new WebApiRequest(url, HttpMethod.Put)
            {
                Model = passwordResetRequest
            };

            var result = GetResponseAs<TokenActionValidationResult>(request);
            return result;
        }

        public PostResetPasswordEmailResponse SendResetPasswordEmail(ClientResetPasswordEmailRequest clientResetPasswordEmailRequest)
        {
            var url = CreateMethodUrl("ResetPasswordEmail");
            var request = new WebApiRequest(url, HttpMethod.Post)
            {
                Model = clientResetPasswordEmailRequest
            };

            var result = GetResponseAs<PostResetPasswordEmailResponse>(request);
            return result;
        }

        public bool UpdateUser(ClientUser user, string targetUserId, string token)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var parameters = new Dictionary<string, object>
            {
                {"targetUserId", targetUserId}
            };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, new HttpMethod("PATCH"), token);
            var success = GetResponseAs<bool>(request);
            return success;
        }

        public AppUser GetUserById(string userId, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"targetUserId", userId}
            };

            var url = CreateRootUrl(parameters);
            var request = new WebApiRequest(url, HttpMethod.Get, token);
            var appUser = GetResponseAs<AppUser>(request);
            return appUser;
        }

        protected override string ControllerName => CoreVersion ? "users" : "User";

        public WebApiUserService(IHttpClientFactory httpClientFactory) : base(httpClientFactory, true)
        {
        }
    }
}