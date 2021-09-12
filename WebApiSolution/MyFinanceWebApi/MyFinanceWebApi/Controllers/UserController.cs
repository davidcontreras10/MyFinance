using System;
using System.Web.Http;
using MyFinanceBackend.Attributes;
using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApi.CustomHandlers;
using Serilog;

namespace MyFinanceWebApi.Controllers
{
	[RoutePrefix("api/user")]
    public class UserController : BaseController
    {
        #region Attributes

        private readonly IUsersService _usersService;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public UserController(IUsersService usersService, ILogger logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        #endregion

        #region Web Methods

        [ResourceActionRequired(ApplicationResources.Users, ResourceActionNames.View)]
        [ValidateModelState]
        [HttpGet]
        [Route]
        public AppUser GetUserById([FromUri]string targetUserId)
        {
            var appUser = _usersService.GetUser(targetUserId);
            return appUser;
        }

        [ResourceActionRequired(ApplicationResources.Users, ResourceActionNames.Edit)]
        [ValidateModelState]
        [HttpPatch]
        [Route]
        public bool UpdateUser([FromUri] string targetUserId, [FromBody] ClientEditUser editUser)
        {
            if (string.IsNullOrEmpty(targetUserId))
            {
                throw new ArgumentNullException(nameof(targetUserId));
            }

            editUser.UserId = targetUserId;
            var userId = GetUserId();
            var result = _usersService.UpdateUser(userId, editUser);
            return result;
        }

        [ValidateModelState]
        [AllowAnonymous]
        [HttpPut]
        [Route("ResetPassword")]
        public TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest)
        {
            var result = _usersService.UpdateUserPassword(passwordResetRequest);
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ResetPassword")]
        public ResetPasswordValidationResult GetResetPasswordValidationResult(string actionLink)
        {
            var result = _usersService.ValidateResetPasswordActionResult(actionLink);
            return result;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPasswordEmail")]
        public PostResetPasswordEmailResponse SendResetPasswordEmail([FromBody] ClientResetPasswordEmailRequest request)
        {
            var valid = _usersService.ValidResetPasswordEmailRequest(request);
            if (!valid)
            {
                return PostResetPasswordEmailResponse.Invalid;
            }

            var result = _usersService.SendResetPasswordEmail(request);
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("InSession")]
		public bool InSession()
        {
            return RequestContext.Principal.Identity.IsAuthenticated;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ResultLoginAttempt")]
        public IHttpActionResult ResultLoginAttempt(string username, string password)
        {
            var loginResult = _usersService.AttemptToLogin(username, password);
            return Ok(loginResult);
        }

        [ResourceActionRequired(ApplicationResources.Users, ResourceActionNames.EditSensitive)]
        [Route("Password")]
        [ValidateModelState]
        [HttpPatch]
        public IHttpActionResult SetUserPassword([FromBody] SetPassword setPassword)
        {
            var result = _usersService.SetPassword(setPassword.UserId, setPassword.NewPassword);
            return Ok(result);
        }

        #endregion
    }
}