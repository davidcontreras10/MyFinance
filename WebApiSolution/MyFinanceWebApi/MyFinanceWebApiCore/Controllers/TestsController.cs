using EFDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceBackend.Data;
using MyFinanceModel;
using System;
using System.Collections.Generic;
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
		public async Task<IEnumerable<BankAccountPeriodBasicId>> TestGetEndpoint([FromQuery]string userId)
		{
			return await _accountRepository.GetBankSummaryAccountsPeriodByUserIdAsync(userId, DateTime.Now);
		}
	}
}
