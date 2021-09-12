﻿using MyFinanceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrencyService.Models
{
    public class ExchangeRateData
    {
        #region Attributes

        public double Purchase { get; set; }
        public ExchangeRateResult.ResultError ErrorType { get; set; }
        public int MethodId { get; set; }
        public double Sell { get; set; }
        public ExchangeRateResult.ResultType ResultTypeValue { get; set; }
        public bool Success { get; set; }

        #endregion
    }
}