using EFDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MyFinanceBackend.Data;
using MyFinanceBackend.Models;
using MyFinanceBackend.ServicesExceptions;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFDataAccess.Repositories
{
	public class EFSpendsRepository : BaseEFRepository, ISpendsRepository
	{
		protected EFSpendsRepository(MyFinanceContext context) : base(context)
		{
		}

		#region Publics

		public IEnumerable<SpendItemModified> AddSpend(ClientAddSpendModel clientAddSpendModel)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SpendItemModified> AddSpend(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId)
		{
			throw new NotImplementedException();
		}

		public void AddSpendDependency(int spendId, int dependencySpendId)
		{
			throw new NotImplementedException();
		}

		public void BeginTransaction()
		{
			throw new NotImplementedException();
		}

		public void Commit()
		{
			throw new NotImplementedException();
		}

		public ClientAddSpendModel CreateClientAddSpendModel(ClientBasicAddSpend clientBasicAddSpend, int accountPeriodId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SpendItemModified> DeleteSpend(string userId, int spendId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SpendItemModified> EditSpend(FinanceSpend financeSpend)
		{
			throw new NotImplementedException();
		}

		public AccountFinanceViewModel GetAccountFinanceViewModel(int accountPeriodId, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<AccountFinanceViewModel> GetAccountFinanceViewModel(IEnumerable<ClientAccountFinanceViewModel> requestItems, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ClientAddSpendAccount> GetAccountMethodConversionInfo(int? accountId, int? accountPeriodId, string userId, int currencyId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<AccountCurrencyPair> GetAccountsCurrency(IEnumerable<int> accountIdsArray)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string userId)
		{
			throw new NotImplementedException();
		}

		public DateRange GetDateRange(string accountIds, DateTime? dateTime, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<EditSpendViewModel> GetEditSpendViewModel(int accountPeriodId, int spendId, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CurrencyViewModel> GetPossibleCurrencies(int accountId, string userId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SavedSpend> GetSavedSpends(int spendId)
		{
			throw new NotImplementedException();
		}

		public SpendActionAttributes GetSpendAttributes(int spendId)
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction()
		{
			throw new NotImplementedException();
		}

		#endregion

		private async Task ValidateSpendCurrencyConvertibleValuesAsync(ISpendCurrencyConvertible spendCurrencyConvertible)
		{
			if (spendCurrencyConvertible == null)
				throw new ArgumentNullException(nameof(spendCurrencyConvertible));
			var accountData =
				spendCurrencyConvertible.IncludedAccounts.Select(
					item => SpendsDataHelper.CreateClientAddSpendCurrencyData(item, spendCurrencyConvertible.CurrencyId));
			var clientAddSpendValidationResultSet = await GetClientAddSpendValidationResultSetAsync(accountData);
			var invalids = clientAddSpendValidationResultSet.Where(item => !item.IsSuccess);
			if (!invalids.Any())
				return;
			var invalidAccounts = invalids.Select(SpendsDataHelper.CreateAccountCurrencyConverterData);
			throw new InvalidAddSpendCurrencyException(invalidAccounts);
		}

		private async Task<IEnumerable<ClientAddSpendValidationResultSet>> GetClientAddSpendValidationResultSetAsync(
			IEnumerable<ClientAddSpendCurrencyData> clientAddSpendCurrencyDataList)
		{
			var accountIds = clientAddSpendCurrencyDataList.Select(c => c.AccountId);
			var accounts = await Context.Account
				.Where(acc => accountIds.Contains(acc.AccountId))
				.ToListAsync();
			var ccmIds = clientAddSpendCurrencyDataList.Select(ccm => ccm.CurrencyConverterMethodId);
			var currencyConverterMethods = await Context.CurrencyConverterMethod
				.Where(ccm => ccmIds.Contains(ccm.CurrencyConverterMethodId))
				.Include(ccm=>ccm.CurrencyConverter)
				.ToListAsync();
			var results = new List<ClientAddSpendValidationResultSet>();
			foreach(var cAccount in clientAddSpendCurrencyDataList)
			{
				var account = accounts.First(acc => acc.AccountId == cAccount.AccountId);
				var ccm = currencyConverterMethods.First(x=>x.CurrencyConverterMethodId  == cAccount.CurrencyConverterMethodId);
				var result = new ClientAddSpendValidationResultSet
				{
					AccountId = account.AccountId,
					AccountName = account.Name,
					AmountCurrencyId = cAccount.AmountCurrencyId,
					ConvertCurrencyId = ccm.CurrencyConverterId,
					CurrencyIdOne = ccm.CurrencyConverter.CurrencyIdOne,
					CurrencyIdTwo = ccm.CurrencyConverter.CurrencyIdTwo,
					IsSuccess = ccm.CurrencyConverter.CurrencyIdOne == cAccount.AmountCurrencyId
						&& ccm.CurrencyConverter.CurrencyIdTwo == account.CurrencyId
				};

				results.Add(result);
			}

			return results;
		}

	}
}
