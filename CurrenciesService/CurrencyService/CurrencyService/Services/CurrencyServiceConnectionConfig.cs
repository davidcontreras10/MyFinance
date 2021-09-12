using System.Configuration;
using DataAccess;

namespace CurrencyService.Services
{
    public class CurrencyServiceConnectionConfig : IConnectionConfig
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ExchangeCurrencyServiceDev"].ConnectionString;
        }

	    public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.ConnectionPooling;
    }
}