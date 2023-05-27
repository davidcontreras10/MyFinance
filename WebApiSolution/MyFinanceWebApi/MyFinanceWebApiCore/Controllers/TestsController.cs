using EFDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceBackend.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestsController : BaseApiController
	{
		private readonly IAccountRepository _accountRepository;

		public TestsController(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository;
		}

		[HttpGet]
		public ActionResult TestGetEndpoint([FromQuery]int[] accountIds, [FromQuery]string userId)
		{
			return Ok(_accountRepository.GetAccountDetailsViewModel(accountIds, userId));
		}
	}
}
