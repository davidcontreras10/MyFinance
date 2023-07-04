using MyFinanceBackend.Models;
using MyFinanceBackend.ServicesExceptions;
using MyFinanceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFinanceBackend.Services
{
	public interface ITrxExchangeService
	{
		Task<IEnumerable<AddSpendAccountDbValues>> ConvertTrxCurrencyAsync(ISpendCurrencyConvertible spendCurrencyConvertible, IReadOnlyCollection<AccountCurrencyPair> accountCurrencyPairList);
	}

	public class TrxExchangeService : ITrxExchangeService
	{
		private readonly ICurrencyService _currencyService;

		public TrxExchangeService(ICurrencyService currencyService)
		{
			_currencyService = currencyService;
		}

		public async Task<IEnumerable<AddSpendAccountDbValues>> ConvertTrxCurrencyAsync(
			ISpendCurrencyConvertible spendCurrencyConvertible,
			IReadOnlyCollection<AccountCurrencyPair> accountCurrencyPairList)
		{
			if (spendCurrencyConvertible == null)
				throw new ArgumentNullException(nameof(spendCurrencyConvertible));
			var accountIds = new List<int> { spendCurrencyConvertible.OriginalAccountData.AccountId };
			accountIds.AddRange(spendCurrencyConvertible.IncludedAccounts.Select(item => item.AccountId));
			var accountModelCurrencyList = CreateAccountModelCurrency(spendCurrencyConvertible, accountCurrencyPairList);
			var addSpendAccountDbValues = await ConvertTrxCurrencyAsync(spendCurrencyConvertible.PaymentDate,
																			spendCurrencyConvertible.CurrencyId,
																			accountModelCurrencyList);
			foreach (var value in addSpendAccountDbValues)
			{
				value.IsOriginal = value.AccountId == spendCurrencyConvertible.OriginalAccountData.AccountId;
			}

			return addSpendAccountDbValues;
		}

		private static IEnumerable<AccountModelCurrency> CreateAccountModelCurrency(ISpendCurrencyConvertible spendCurrencyConvertible,
																	 IEnumerable<AccountCurrencyPair> pairs)
		{
			if (spendCurrencyConvertible == null)
				throw new ArgumentNullException(nameof(spendCurrencyConvertible));
			if (pairs == null || !pairs.Any())
				throw new ArgumentException(@"Value cannot be null or empty", nameof(pairs));
			var accountModelCurrencyList = new List<AccountModelCurrency>
				{
					new AccountModelCurrency
						{
							AccountInfo = spendCurrencyConvertible.OriginalAccountData,
							AccountOriginalCurrencyId =
								pairs.First(item => item.AccountId == spendCurrencyConvertible.OriginalAccountData.AccountId)
									 .CurrencyId
						}
				};
			accountModelCurrencyList.AddRange(
				spendCurrencyConvertible.IncludedAccounts.Select(clientAddSpendAccount => new AccountModelCurrency
				{
					AccountInfo = clientAddSpendAccount,
					AccountOriginalCurrencyId =
							pairs.First(item => item.AccountId == clientAddSpendAccount.AccountId).CurrencyId
				}));
			return accountModelCurrencyList;
		}

		private async Task<IEnumerable<AddSpendAccountDbValues>> ConvertTrxCurrencyAsync(DateTime dateTime, int amountCurrency,
																			   IEnumerable<AccountModelCurrency>
																				   accountModelCurrencies)
		{
			var dbValues = new List<AddSpendAccountDbValues>();
			dbValues.AddRange(
				accountModelCurrencies.Where(item => item.AccountOriginalCurrencyId == amountCurrency)
									  .Select(
										  item2 =>
										  CreateAddSpendAccountDbValues(item2,
																		ExchangeRateResult
																			.CreateDefaultExchangeRateResult())));
			var methodIds =
				accountModelCurrencies.Where(
					item => dbValues.All(item2 => item2.AccountId != item.AccountInfo.AccountId))
									  .Select(item3 => item3.AccountInfo.ConvertionMethodId);//TODO validate methodId not to be repeated
			if (!methodIds.Any())
				return dbValues;
			var exchangeRateResultList = await _currencyService.GetExchangeRateResultAsync(methodIds, dateTime);
			dbValues.AddRange(
				accountModelCurrencies.Where(
					item => dbValues.All(item2 => item2.AccountId != item.AccountInfo.AccountId))
					.Select(
						item3 =>
							CreateAddSpendAccountDbValues(item3,
								exchangeRateResultList.First(
									item4 =>
										item4.MethodId ==
										item3.AccountInfo.ConvertionMethodId)))
				);
			return dbValues;
		}
		private static AddSpendAccountDbValues CreateAddSpendAccountDbValues(AccountModelCurrency accountModelCurrency,
																	 ExchangeRateResult exchangeRateResult)
		{
			if (accountModelCurrency == null)
				throw new ArgumentNullException(nameof(accountModelCurrency));
			if (exchangeRateResult == null || !exchangeRateResult.Success)
				throw new InvalidExchangeRateCreationException("Cannot create value with invalid exchangeRateResult");
			return new AddSpendAccountDbValues
			{
				AccountId = accountModelCurrency.AccountInfo.AccountId,
				PendingUpdate = exchangeRateResult.ResultTypeValue == ExchangeRateResult.ResultType.PendingUpdate,
				Numerator = exchangeRateResult.Numerator,
				Denominator = exchangeRateResult.Denominator,
				CurrencyConverterMethodId = accountModelCurrency.AccountInfo.ConvertionMethodId
			};
		}
	}
}
