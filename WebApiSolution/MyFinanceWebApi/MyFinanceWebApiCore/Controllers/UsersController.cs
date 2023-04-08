using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Attributes;
using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : BaseApiController
	{
		private readonly IUsersService _usersService;

		public UsersController(IUsersService usersService)
		{
			_usersService = usersService;
		}

		#region Web Methods

		[ResourceActionRequired(ApplicationResources.Users, ResourceActionNames.View)]
		[HttpGet]
		public AppUser GetUserById([FromQuery] string targetUserId)
		{
			var appUser = _usersService.GetUser(targetUserId);
			return appUser;
		}

		[ResourceActionRequired(ApplicationResources.Users, ResourceActionNames.Edit)]
		[HttpPatch]
		public bool UpdateUser([FromQuery] string targetUserId, [FromBody] ClientEditUser editUser)
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
			return Request.HttpContext.User.Identity.IsAuthenticated;
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("ResultLoginAttempt")]
		public LoginResult ResultLoginAttempt(string username, string password)
		{
			return _usersService.AttemptToLogin(username, password);
		}

		[ResourceActionRequired(ApplicationResources.Users, ResourceActionNames.EditSensitive)]
		[Route("Password")]
		[HttpPatch]
		public bool SetUserPassword([FromBody] SetPassword setPassword)
		{
			return _usersService.SetPassword(setPassword.UserId, setPassword.NewPassword);
		}

		#endregion
	}
}
