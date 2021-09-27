using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MyFinance.MyFinanceWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyFinance.MyFinanceWebApp.Services;

namespace MyFinance.MyFinanceWebApp.Controllers
{
	public class AuthController : Controller
	{
		private const string CredCookieName = "myFnCrd";

		private readonly IUserService _userService;
		private readonly IAppSettings _appSettings;
		private readonly ILogger<AuthController> _logger;

		public AuthController(IUserService userService, IAppSettings appSettings, ILogger<AuthController> logger)
		{
			_userService = userService;
			_appSettings = appSettings;
			_logger = logger;
		}

		[AllowAnonymous]
		public IActionResult LogOff()
		{
			ClearCredentialsCookie();
			return View("Login");
		}

		[RequireHttps]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl)
		{
			TempData["returnUrl"] = returnUrl;
			var cookieAuthAttempt = await CookieAuthenticateAttemptAsync();
			if (cookieAuthAttempt == CookieAuthResult.Invalid)
			{
				ClearCredentialsCookie();
			}
			else
			{
				returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
				return Redirect(returnUrl);
			}

			return View("Login");
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
		{
			try
			{
				var success = await AuthenticateAttemptAsync(model);
				if (!success)
				{
					ClearCredentialsCookie();
					TempData["returnUrl"] = returnUrl;
					return View("Login");
				}

				if (string.IsNullOrWhiteSpace(returnUrl))
				{
					return RedirectToAction(returnUrl);
				}
				else
				{
					return RedirectToAction("Index", "Home");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"[UserController].[AuthenticateUser] Error: {ex}");
				return View("Login");
			}

		}

		private async Task<CookieAuthResult> CookieAuthenticateAttemptAsync()
		{
			var credCookie = HttpContext.Request.Cookies.TryGetValue(CredCookieName, out var val) ? 
				val : string.Empty;
			if (string.IsNullOrWhiteSpace(credCookie))
			{
				return CookieAuthResult.Invalid;
			}

			var sessionBasics = SessionBasics.ReadFromKey(credCookie);
			if (!sessionBasics.HasExpired())
			{
				var tokenLoginAttempt = await ValidTokenLoginAttemptAsync(sessionBasics);
				if (tokenLoginAttempt)
				{
					WriteCredentialsCookie(sessionBasics);
					return CookieAuthResult.NoRefresh;
				}
			}

			var cookieCredentialsAttempt = await CookieCredentialsLoginAttempt(sessionBasics);
			if (cookieCredentialsAttempt)
			{
				return CookieAuthResult.TokenRefresh;
			}

			return CookieAuthResult.Invalid;
		}

		private async Task<bool> CookieCredentialsLoginAttempt(SessionBasics sessionBasics)
		{
			var loginModel = new LoginModel
			{
				Password = sessionBasics.Password,
				RememberMe = true,
				Username = sessionBasics.Username
			};

			return await AuthenticateAttemptAsync(loginModel);
		}

		private async Task<bool> ValidTokenLoginAttemptAsync(SessionBasics sessionBasics)
		{
			var tokenValid = await _userService.IsSessionValidAsync(sessionBasics.Jwt);
			if (!tokenValid)
			{
				return false;
			}

			await InternalAuthenticationAsync(sessionBasics.Username, sessionBasics.UserId, sessionBasics.Jwt);
			return true;
		}

		private async Task<bool> AuthenticateAttemptAsync(LoginModel model)
		{
			var tokenObject = await _userService.GetAuthTokenAsync(model.Username, model.Password);
			if (tokenObject == null)
			{
				return false;
			}

			var userDetails = await _userService.AttemptLoginAsync(model.Username, model.Password);
			if (userDetails is not {IsAuthenticated: true})
			{
				return false;
			}

			await InternalAuthenticationAsync(userDetails.User.Username, userDetails.User.UserId.ToString(),
				tokenObject.AccessToken);
			var tokenExpires = DateTime.Now.AddSeconds(tokenObject.ExpiresIn);
			if (model.RememberMe)
			{
				WriteCredentialsCookie(new SessionBasics
				{
					Username = userDetails.User.Username,
					UserId = userDetails.User.UserId.ToString(),
					Password = model.Password,
					Jwt = tokenObject.AccessToken,
					TokenExpires = tokenExpires
				});
			}

			return true;
		}

		private async Task InternalAuthenticationAsync(string username, string userId, string accessToken)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.UserData, username),
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Authentication, accessToken)
			};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(identity));
		}

		private void WriteCredentialsCookie(SessionBasics sessionBasics)
		{
			var options = new CookieOptions()
			{
				Expires = new DateTimeOffset(DateTime.Now.Add(_appSettings.WebAuthExpireTime)),
				Secure = true
			};

			HttpContext.Response.Cookies.Append(CredCookieName, sessionBasics.GetAsCookieValue(), options);
		}

		private void ClearCredentialsCookie()
		{
			HttpContext.Response.Cookies.Delete(CredCookieName);
			HttpContext.SignOutAsync();
		}

		private enum CookieAuthResult
		{
			Invalid = 0,
			NoRefresh = 1,
			TokenRefresh = 2
		}
	}
}
