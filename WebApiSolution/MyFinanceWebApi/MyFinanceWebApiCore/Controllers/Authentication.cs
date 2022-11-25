using Microsoft.AspNetCore.Mvc;
using MyFinanceWebApiCore.Models;
using MyFinanceWebApiCore.Services;
using System.Threading.Tasks;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Authentication : ControllerBase
	{
		private readonly IAuthenticationService _authenticationService;

		public Authentication(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		[HttpPost]
		public async Task<AuthToken> AuthenticateAsync(AuthenticateRequest authenticateRequest)
		{
			return await _authenticationService.AuthenticateAsync(authenticateRequest);
		}
	}
}
