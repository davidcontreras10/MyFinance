using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyFinance.MyFinanceWebApp.Controllers
{
	public class AppBaseController : Controller
	{
		protected string GetUserToken()
		{
			return HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Authentication).Value;
		}
	}
}
