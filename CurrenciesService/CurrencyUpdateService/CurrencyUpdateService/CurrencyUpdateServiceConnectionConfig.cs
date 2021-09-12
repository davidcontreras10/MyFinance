using System.Configuration;
using DataAccess;

namespace CurrencyUpdateService
{
    public class CurrencyUpdateServiceConnectionConfig : IConnectionConfig
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ExchangeCurrencyServiceDev"].ConnectionString;
        }
    }
}
