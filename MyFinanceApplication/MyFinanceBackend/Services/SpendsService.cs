using System;
using System.Collections.Generic;
using System.Linq;
using MyFinanceBackend.Data;
using MyFinanceBackend.Models;
using MyFinanceBackend.ServicesExceptions;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
{
	public class SpendsService : ISpendsService
	{
		#region Constructor

		public SpendsService(ISpendsRepository spendsRepository, IResourceAccessRepository resourceAccessRepository)
		{
			_spendsRepository = spendsRepository;
			_resourceAccessRepository = resourceAccessRepository;
		}

		#endregion

		#region Attributes

		private readonly ISpendsRepository _spendsRepository;
		private readonly IResourceAccessRepository _resourceAccessRepository;

		#endregion

		#region Public Methods

		public SpendActionResult GetSpendActionResult(int spendId, ResourceActionNames actionType, ApplicationModules applicationModule)
		{
			if (actionType == ResourceActionNames.Unknown)
			{
				throw new ArgumentException(nameof(actionType));
			}

			if (applicationModule == ApplicationModules.Unknown)
			{
				throw new ArgumentException(nameof(applicationModule));
			}

			var spendAttributes = _spendsRepository.GetSpendAttributes(spendId);
			var applicationResource = ApplicationResources.Spends;
			var resourcesAccessResponse = _resourceAccessRepository.GetResourceAccessReport(applicationResourceId: (int)applicationResource,
				applicationModuleId: (int)applicationModule, resourceActionId: (int)actionType, resourceAccessLevelId: null);
			var response = CreateSpendActionResult(spendAttributes, resourcesAccessResponse, actionType);
			return response;
		}

		public IEnumerable<AddSpendViewModel> GetAddSpendViewModel(IEnumerable<int> accountPeriodIds, string userId)
		{
			return _spendsRepository.GetAddSpendViewModel(accountPeriodIds, userId);
		}

		public IEnumerable<EditSpendViewModel> GetEditSpendViewModel(int accountPeriodId, int spendId, string userId)
		{
			return _spendsRepository.GetEditSpendViewModel(accountPeriodId, spendId, userId);
		}

		public IEnumerable<SavedSpend> GetSavedSpends(int spendId)
		{
			return _spendsRepository.GetSavedSpends(spendId);
		}

		public IEnumerable<SpendItemModified> AddBasicTransaction(ClientBasicTrxByPeriod clientBasicTrxByPeriod, TransactionTypeIds transactionTypeId)
		{
			if (clientBasicTrxByPeriod.Amount <= 0)
				throw new InvalidAmountException();

			if (transactionTypeId == TransactionTypeIds.Invalid || transactionTypeId == TransactionTypeIds.Ignore)
			{
				throw new ArgumentException($"{nameof(transactionTypeId)} cannot be invalid");
			}

			var clientAddSpendModel =
				_spendsRepository.CreateClientAddSpendModel(clientBasicTrxByPeriod,
					clientBasicTrxByPeriod.AccountPeriodId);
			return transactionTypeId == TransactionTypeIds.Saving ? AddIncome(clientAddSpendModel) : AddSpend(clientAddSpendModel);
		}

		public IEnumerable<SpendItemModified> AddIncome(ClientAddSpendModel clientAddSpendModel)
		{
			if (clientAddSpendModel.Amount <= 0)
				throw new InvalidAmountException();
			clientAddSpendModel.AmountTypeId = TransactionTypeIds.Saving;
			var result = _spendsRepository.AddSpend(clientAddSpendModel);
			return result;
		}

		public IEnumerable<SpendItemModified> AddSpend(ClientAddSpendModel clientAddSpendModel)
		{
			if (clientAddSpendModel == null)
				throw new ArgumentNullException(nameof(clientAddSpendModel));
			if (clientAddSpendModel.Amount <= 0)
				throw new ArgumentException("Amount must be greater than zero");
			clientAddSpendModel.AmountTypeId = TransactionTypeIds.Spend;
			var result = _spendsRepository.AddSpend(clientAddSpendModel);
			return result;
		}

		public IEnumerable<SpendItemModified> DeleteSpend(string userId, int spendId)
		{
			var result = _spendsRepository.DeleteSpend(userId, spendId);
			return result;
		}

		public DateRange GetDateRange(string accountIds, DateTime? dateTime, string userId)
		{
			var result = _spendsRepository.GetDateRange(accountIds, dateTime, userId);
			return result;
		}

		public IEnumerable<SpendItemModified> EditSpend(ClientEditSpendModel model)
		{
			return _spendsRepository.EditSpend(model);
		}

		public IEnumerable<AccountCurrencyPair> GetAccountsCurrency(IEnumerable<int> accountIdsArray)
		{
			var result = _spendsRepository.GetAccountsCurrency(accountIdsArray);
			return result;
		}

		public IEnumerable<SpendItemModified> ConfirmPendingSpend(int spendId, DateTime newPaymentDate)
		{
			var spends = _spendsRepository.GetSavedSpends(spendId);
			if (spends == null || !spends.Any())
			{
				return new SpendItemModified[0];
			}

			var modifiedList = new List<SpendItemModified>();
			_spendsRepository.BeginTransaction();
			try
			{
				foreach (var savedSpend in spends)
				{
					var financeSpend = CreateFinanceSpend(savedSpend, newPaymentDate);
					if (!savedSpend.IsPending)
					{
						throw new SpendNotPendingException(financeSpend.SpendId);
					}

					var modifiedItems = _spendsRepository.EditSpend(financeSpend);
					modifiedList.AddRange(modifiedItems);
				}
				_spendsRepository.Commit();
			}
			catch (Exception)
			{
				_spendsRepository.RollbackTransaction();
				throw;
			}

			return modifiedList;
		}

		#endregion

		#region Privates

		private static FinanceSpend CreateFinanceSpend(SavedSpend savedSpend, DateTime newDateTime)
		{
			if (savedSpend == null)
			{
				throw new ArgumentNullException(nameof(savedSpend));
			}

			var result = new FinanceSpend
			{
				SpendId = savedSpend.SpendId,
				Amount = savedSpend.Amount,
				UserId = savedSpend.UserId,
				SpendDate = savedSpend.SpendDate,
				AmountDenominator = savedSpend.AmountDenominator,
				CurrencyId = savedSpend.CurrencyId,
				AmountNumerator = savedSpend.AmountNumerator,
				SetPaymentDate = newDateTime,
				OriginalAccountData = savedSpend.OriginalAccountData,
				IncludedAccounts = savedSpend.IncludedAccounts,
				IsPending = false
			};

			return result;
		}

		private static SpendActionResult CreateSpendActionResult(SpendActionAttributes spendActionAttributes, 
			IEnumerable<ResourceAccessReportRow> resourceAccessReportRows, ResourceActionNames resourceActionNames)
		{
			if(spendActionAttributes == null)
			{
				throw new ArgumentNullException(nameof(spendActionAttributes));
			}

			if(resourceAccessReportRows == null)
			{
				throw new ArgumentNullException(nameof(resourceAccessReportRows));
			}

			var response = new SpendActionResult
			{
				Action = resourceActionNames,
				IsLoan = spendActionAttributes.IsLoan,
				IsTransfer = spendActionAttributes.IsTransfer,
				SpendId = spendActionAttributes.SpendId
			};

			var isValid = resourceAccessReportRows.Any(r => 
				spendActionAttributes.AccessLevels.Any(r2 => (int)r2 == r.ResourceAccessLevelId));
			if (isValid)
			{
				response.Result = SpendActionAttributes.ActionResult.Valid;
				return response;
			}

			response.Result = SpendActionAttributes.ActionResult.Unknown;
			return response;
		}

		#endregion
	}
}