using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MyFinanceWebApiCore.Controllers
{
	public class BaseApiController : ControllerBase
	{
		protected string GetUserId()
		{
			var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
			return userIdClaim.Value;
		}
	}
}
