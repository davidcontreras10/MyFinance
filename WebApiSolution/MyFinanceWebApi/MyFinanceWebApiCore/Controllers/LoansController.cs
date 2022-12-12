using Microsoft.AspNetCore.Mvc;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceModel;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MyFinanceWebApiCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoansController : BaseApiController
	{
		#region Attributes

		private readonly ILoanService _loanService;

		#endregion

		#region Constructor

		public LoansController(ILoanService loanService)
		{
			_loanService = loanService;
		}

		#endregion

		#region Api

		[HttpDelete]
		public IEnumerable<ItemModified> DeleteLoan(int loanRecordId)
		{
			var userId = GetUserId();
			var result = _loanService.DeleteLoan(loanRecordId, userId);
			return result;
		}

		[HttpPost]
		public IEnumerable<SpendItemModified> CreateLoan(ClientLoanViewModel clientLoanViewModel)
		{
			if (clientLoanViewModel == null)
			{
				throw new ArgumentNullException(nameof(clientLoanViewModel));
			}

			clientLoanViewModel.UserId = GetUserId();
			var response = _loanService.CreateLoan(clientLoanViewModel);
			return response;
		}

		[Route("payment")]
		[HttpPost]
		public IEnumerable<SpendItemModified> AddPayment(ClientLoanSpendViewModel clientLoanSpendViewModel)
		{
			if (clientLoanSpendViewModel == null)
			{
				throw new ArgumentNullException(nameof(clientLoanSpendViewModel));
			}

			clientLoanSpendViewModel.UserId = GetUserId();
			var response = _loanService.AddLoanSpend(clientLoanSpendViewModel);
			return response;
		}

		[Route("accounts")]
		[HttpGet]
		public IEnumerable<AccountDetailsViewModel> GetSupportedLoanAccount()
		{
			var userId = GetUserId();
			var result = _loanService.GetSupportedLoanAccount(userId);
			return result;
		}

		[Route("add")]
		[HttpGet]
		public AddLoanRecordViewModel GetAddLoanRecordViewModel(int accountId, DateTime dateTime)
		{
			if (accountId == 0)
			{
				throw new ArgumentException(nameof(accountId));
			}

			var userId = GetUserId();
			var response = _loanService.GetAddLoanRecordViewModel(dateTime, accountId, userId);
			return response;
		}

		[Route("add/payment")]
		[HttpGet]
		public AddLoanSpendViewModel GetAddLoanSpendViewModel(int loanRecordId)
		{
			if (loanRecordId == 0)
			{
				throw new ArgumentException(nameof(loanRecordId));
			}

			var userId = GetUserId();
			var response = _loanService.GetAddLoanSpendViewModel(loanRecordId, userId);
			return response;
		}

		[Route("{loanRecordId}")]
		[HttpGet]
		public LoanReportViewModel GetLoanReportViewModelByLoanRecordId(int loanRecordId)
		{
			var response = _loanService.GetLoanDetailRecordsByIds(new[] { loanRecordId });
			return response.FirstOrDefault();
		}

		[HttpGet]
		public IEnumerable<LoanReportViewModel> GetLoanDetailRecordsByCriteriaId([FromQuery] int loanRecordStatusId, [FromQuery] LoanQueryCriteria criteriaId, [FromQuery] int[] ids = null)
		{
			var userId = GetUserId();
			var result = _loanService.GetLoanDetailRecordsByCriteriaId(userId, loanRecordStatusId, criteriaId, ids, ids);
			return result;
		}

		[HttpGet]
		[Route("destinationAccounts")]
		public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountId, DateTime dateTime,
			int currencyId)
		{
			var userId = GetUserId();
			var result = _loanService.GetPossibleDestinationAccount(accountId, dateTime, userId, currencyId);
			return result;
		}

		#endregion
	}
}
