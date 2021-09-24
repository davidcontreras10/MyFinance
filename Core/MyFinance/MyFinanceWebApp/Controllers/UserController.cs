using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MyFinance.MyFinanceWebApp.Constants;
using MyFinance.MyFinanceWebApp.Models;
using Microsoft.AspNetCore.Authentication;

namespace MyFinance.MyFinanceWebApp.Controllers
{
	public class UserController : Controller
	{
		[RequireHttps]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl)
		{
			TempData["returnUrl"] = returnUrl;
			return View("Login");
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
		{
			try
			{
				var claims = new[] {new Claim(ClaimTypes.Name, model.Username)};
				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(identity));
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				Debug.WriteLine("[UserController].[AuthenticateUser] Error: " + ex);
				return View("Login");
			}

		}
    }
}
