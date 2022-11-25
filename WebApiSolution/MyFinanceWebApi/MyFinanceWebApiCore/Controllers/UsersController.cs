using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Services;
using MyFinanceModel;
using System.Threading.Tasks;

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

		[HttpGet]
		public async Task<AppUser> Get()
		{
			var userId = GetUserId();
			return await _usersService.GetUserAsync(userId);
		} 
	}
}
