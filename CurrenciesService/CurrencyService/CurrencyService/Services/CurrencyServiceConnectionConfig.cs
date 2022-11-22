using System.Configuration;
using DataAccess;

namespace Domain.Services
{
    public class CurrencyServiceConnectionConfig : IConnectionConfig
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ExchangeCurrencyService"].ConnectionString;
        }

	    public ConnectionAdminModes ConnectionAdminMode => ConnectionAdminModes.ConnectionPooling;
    }
}