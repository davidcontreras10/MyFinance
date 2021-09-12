using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyUpdateService
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
}
