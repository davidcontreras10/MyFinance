using MyFinanceBackend.Services;


namespace MyFinanceWebService
{
    internal class BackendInstance
    {
        #region Constructor

        public BackendInstance(string connectionName)
        {
        }

        #endregion

        #region Properties

        public SpendsService Spends { set; get; }
        public UsersService Users { set; get; }
        public ViewDataService ViewData { get; set; }
        public AccountsPeriodsService AccountPeriods { get; set; }
        public AccountService Accounts { get; set; }

        #endregion
    }
}