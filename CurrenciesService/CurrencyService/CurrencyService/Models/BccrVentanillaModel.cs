using System;

namespace CurrencyService.Models
{
    public class BccrVentanillaModel
    {
        #region Attributes

        public string EntityName { get; set; }
        public float Purchase { get; set; }
        public float Sell { get; set; }
        public DateTime LastUpdate { get; set; }

        #endregion
    }

    public class BccrSingleVentanillaModel
    {
        public string EntityName { get; set; }
        public float Value { get; set; }
        public DateTime LastUpdate { get; set; }
    }

}