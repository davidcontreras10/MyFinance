﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using MyFinanceModel;

namespace Domain.Services
{
	public interface IDolarColonesBccrService
	{
		Task<ExchangeRateResult> GetExchangeRateResultByMethodIdAsync(int methodId, DateTime dateTime);
	}

	public class DolarColonesBccrService : IDolarColonesBccrService
	{
        #region Constructors

        public DolarColonesBccrService(IExchangeCurrencyDataService exchangeCurrencyDataService)
        {
	        _exchangeCurrencyDataService = exchangeCurrencyDataService;
        }

        #endregion

        #region Attributes

        private readonly IExchangeCurrencyDataService _exchangeCurrencyDataService;

        #endregion

        #region Public Method

        public async Task<ExchangeRateResult> GetExchangeRateResultByMethodIdAsync(int methodId, DateTime dateTime)
        {
            var entityMethodInfo = _exchangeCurrencyDataService.GetEntityMethodInfo(methodId);
            return entityMethodInfo == null
                ? ExchangeRateResult.CreateNotImplementedExchangeRateResult(methodId)
                : await GetExchangeRateResultByMethodIdAsync(entityMethodInfo, dateTime);
        }


        #endregion

        #region Private Methods

        private async Task<ExchangeRateResult> GetExchangeRateResultByMethodIdAsync(EntityMethodInfo entityMethodInfo, DateTime dateTime)
        {
            if (entityMethodInfo == null)
                throw new ArgumentNullException("entityMethodInfo");

            if (entityMethodInfo.EntitySearchKey != null && entityMethodInfo.EntitySearchKey == "DEFAULT")
            {
                return ExchangeRateResult.CreateDefaultExchangeRateResult();
            }

            return entityMethodInfo.Colones
                ? await GetColToDolExchangeRateResultAsync(dateTime, entityMethodInfo.EntitySearchKey)
                : await GetDolToColExchangeRateResultAsync(dateTime, entityMethodInfo.EntitySearchKey);
        }

        private async Task<ExchangeRateResult> GetColToDolExchangeRateResultAsync(DateTime dateTime, string entityName)
        {
            var exchangeRateData = await GetExchangeRateResultAsync(dateTime, entityName);
            if (exchangeRateData == null || !exchangeRateData.Success)
            {
                return CreateErrorExchangeRateResult(exchangeRateData);
            }
            const int numerator = 1;
            var denominator = exchangeRateData.Purchase;
            return new ExchangeRateResult
            {
                Denominator = denominator,
                Numerator = numerator,
                MethodId = exchangeRateData.MethodId,
                Success = true,
                ResultTypeValue = exchangeRateData.ResultTypeValue,
                ErrorType = exchangeRateData.ErrorType
            };
        }

        public async Task<ExchangeRateResult> GetDolToColExchangeRateResultAsync(DateTime dateTime, string entityName)
        {
            var exchangeRateData = await GetExchangeRateResultAsync(dateTime, entityName);
            if (exchangeRateData == null || !exchangeRateData.Success)
            {
                return CreateErrorExchangeRateResult(exchangeRateData);
            }
            var numerator = exchangeRateData.Sell;
            const int denominator = 1;
            return new ExchangeRateResult
            {
                Denominator = denominator,
                Numerator = numerator,
                MethodId = exchangeRateData.MethodId,
                Success = true,
                ResultTypeValue = exchangeRateData.ResultTypeValue,
                ErrorType = exchangeRateData.ErrorType
            };
        }

        private async Task<ExchangeRateData> GetExchangeRateResultAsync(DateTime datetime, string entityName)
        {
            var exchangeResults = await _exchangeCurrencyDataService.GetBccrVentanillaModelAsync(entityName, datetime);
            if (exchangeResults == null || !exchangeResults.Any())
            {
	            return new ExchangeRateData
	            {
		            ErrorType = ExchangeRateResult.ResultError.EntityHasNoResults,
		            ResultTypeValue = ExchangeRateResult.ResultType.Error,
		            Success = false
	            };
            }

            if (IsTooEarly(exchangeResults, datetime))
            {
                return new ExchangeRateData
                {
                    ErrorType = ExchangeRateResult.ResultError.TooEarly,
                    ResultTypeValue = ExchangeRateResult.ResultType.Error,
                    Success = false
                };
            }
            if (IsAfterToday(exchangeResults, datetime))
            {
                return new ExchangeRateData
                {
                    ErrorType = ExchangeRateResult.ResultError.AfterToday,
                    ResultTypeValue = ExchangeRateResult.ResultType.Error,
                    Success = false
                };
            }
            var match = GetBccrVentanillaModelMatch(exchangeResults, datetime);
            if (match == null)
            {
                return new ExchangeRateData
                {
                    ErrorType = ExchangeRateResult.ResultError.Unknown,
                    Success = false,
                    ResultTypeValue = ExchangeRateResult.ResultType.Error
                };
            }
            return new ExchangeRateData
            {
                ResultTypeValue = ExchangeRateResult.ResultType.Success,
                Sell = match.Sell,
                Purchase = match.Purchase,
                Success = true
            };

        }

        private BccrVentanillaModel GetBccrVentanillaModelMatch(IEnumerable<BccrVentanillaModel> bccrVentanillaModels,
            DateTime dateTime)
        {
            if (bccrVentanillaModels == null || !bccrVentanillaModels.Any())
                return null;
            bccrVentanillaModels = bccrVentanillaModels.OrderByDescending(item => item.LastUpdate);
            var match = bccrVentanillaModels.FirstOrDefault(item => dateTime >= item.LastUpdate);
            return match;
        }

        private static bool IsAfterToday(IEnumerable<BccrVentanillaModel> list, DateTime dateTime)
        {
            if (list == null || !list.Any())
            {
                return dateTime > DateTime.Today;
            }
            var orderedList = list.OrderByDescending(item => item.LastUpdate);
            var latest = orderedList.ElementAt(0);
            var latestDate = DateTime.Today.AddHours(latest.LastUpdate.Hour + 1).AddDays(1);
            return dateTime > latestDate;
        }

        private static bool IsTooEarly(IEnumerable<BccrVentanillaModel> list, DateTime dateTime)
        {
            if (list == null || !list.Any())
                return false;
            var orderedList = list.OrderBy(item => item.LastUpdate);
            var firstElement = orderedList.ElementAt(0);
            return dateTime < firstElement.LastUpdate;
        }

        private static ExchangeRateResult CreateErrorExchangeRateResult(ExchangeRateData exchangeRateData)
        {
            return new ExchangeRateResult
            {
                ErrorType = exchangeRateData?.ErrorType ?? ExchangeRateResult.ResultError.Unknown,
                Success = false,
                MethodId = exchangeRateData?.MethodId ?? 0,
                ResultTypeValue = exchangeRateData?.ResultTypeValue ?? ExchangeRateResult.ResultType.Error
            };
        }


        #endregion
    }
}