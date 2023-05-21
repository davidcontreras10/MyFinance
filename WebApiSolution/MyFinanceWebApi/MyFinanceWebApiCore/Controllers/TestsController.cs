using EFDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestsController : BaseApiController
	{
		private static readonly MyFinanceContext _context = new MyFinanceContext();

		[HttpGet]
		public async Task<ActionResult<MyFinanceModel.AppUser>> GetUsers()
		{
			var users = await _context.AppUser.ToListAsync();
			var dtoUsers = users.Select(u => new MyFinanceModel.AppUser
			{
				Name = u.Name,
				PrimaryEmail = u.PrimaryEmail,
				UserId = u.UserId,
				Username = u.Username
			});
			return Ok(dtoUsers);
		}
	}
}
