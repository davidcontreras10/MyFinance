using System;

namespace CurrencyUpdateService
{
    public class ExchangeRateModel
    {
        #region Properties

        public double Purchase { get; set; }
        public double Sell { get; set; }
        public DateTime LastUpdate { get; set; }
        public int AccountCurrency { get; set; }

        #endregion
    }
}
