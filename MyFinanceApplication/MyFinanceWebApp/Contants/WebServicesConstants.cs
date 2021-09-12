namespace MyFinanceWebApp.Contants
{
    public static class WebServicesConstants
    {
        #region Service Names

        public const string SPEND_SERVICE_NAME = "MyFinanceSpendWebService.asmx";
        public const string ACCOUNT_SERVICE_NAME = "MyFinanceAccountWebService.asmx";
        public const string USER_SERVICE_NAME = "MyFinanceUserWebService.asmx";
        
        #endregion

        #region Method Names

        public const string GET_SPENDS_INFO_METHOD = "GetSpendsInfo";
        public const string GET_SPENDS_BY_PERIOD_METHOD = "GetSpendsByPeriod";
        public const string ADD_SPEND_METHOD = "AddSpend";
        public const string ADD_SPEND_BY_ACCOUNT_METHOD = "AddSpendByAccount";
        public const string GET_SPEND_TYPES_METHOD = "GetSpendTypes";
	    public const string IN_SESSION = "InSession";
        public const string RESULT_LOGIN_METHOD = "ResultLoginAttempt";
        public const string DELETE_SPEND_METHOD = "DeleteSpend";
        public const string GET_MAIN_VIEW_METHOD = "GetMainViewModelData";
        public const string ADD_PERIOD_METHOD = "AddPeriod";
        public const string GET_ADD_PERIOD_DATA_METHOD = "GetAddPeriodData";
        public const string EDIT_SPEND_METHOD = "EditSpend";
        public const string GET_DATE_RANGE = "GetDateRange";
        public const string GET_SPEND_DETAIL_DATA = "GetSpendDetailData";
        public const string GET_ACCOUNT_DETAIL_SOURCE = "GetAccountDetailSource";
        public const string GET_ACCOUNT_LIST = "GetAccountList";
		public const string GET_TRANSFER_POSSIBLE_CURRENCIES = "possibleCurrencies";
		public const string GET_TRANSFER_POSSIBLE_DESTINATION_ACCOUNTS = "possibleDestination";
		public const string GET_TRANSFER_ACCOUNT_BASIC_INFO = "basicAccountInfo";
        public const string SUBMIT_TRANSFER = "CreateTransfer";
        public const string GET_ACCOUNT_DETAILS_VIEW_MODEL = "GetAccountDetailsViewModel";
        public const string GET_ACCOUNT_INCLUDE_VIEW_MODEL = "include";
        public const string GET_ACCOUNT_DETAILS_INFO_VIEW_MODEL = "GetAccountDetailsInfoViewModel";
		public const string UPDATE_ACCOUNT_POSITIONS = "positions";
		public const string UPDATE_ACCOUNT = "UpdateAccount";
        public const string GET_ADD_ACCOUNT_VIEW_MODEL = "add";

        #endregion
    }
}