using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Script.Serialization;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.CustomHandlers;
using Newtonsoft.Json;

namespace MyFinanceWebApp.Models
{
    public class EditAppUserClientViewModel : BaseModel
    {
        #region Attributes 

        [Required]
        public string Username { set; get; }

        [Required]
        public string Name { set; get; }

        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",ErrorMessage = "Invalid {0} format.")]
        [Display(Name = "Email")]
        [Required]
        public string PrimaryEmail { get; set; }

        [Required]
        [DateRange("1910-1-1")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BirthDate { get; set; }

        #endregion
    }

    public class AuthToken
	{
		[JsonProperty(PropertyName = "access_token")]
		public string AccessToken { get; set; }

		[JsonProperty(PropertyName = "expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty(PropertyName = "refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty(PropertyName = "token_type")]
		public string TokenType { get; set; }
	}

    public class ExecuteTaskRequest
    {
        [Required]
	    public DateTime DateTime { get; set; }
	    
        [Required]
	    public string TaskId { get; set; }
    }

	public class MvcNumber
	{
		public string Value { get; set; }

		public float? GetValue()
		{
			return
				float.TryParse(Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value)
					? value
					: (float?) null;
		}
	}

    public class MvcCustomPeriodAccountData : ClientAccountFinanceViewModel
    {
        public IEnumerable<AccountFinanceViewModel> AccountFinanceViewModels { get; set; }
    }

	public class MvcClientAddAccount
	{
		public string AccountName { get; set; }
		public int PeriodDefinitionId { get; set; }
		public int CurrencyId { get; set; }
		public MvcNumber BaseBudget { get; set; }
		public FrontStyleData HeaderColor { get; set; }
		public FullAccountInfoViewModel.AccountType AccountTypeId { get; set; }
		public int? SpendTypeId { get; set; }
		public int? FinancialEntityId { get; set; }
		public int AccountGroupId { get; set; }
		public IEnumerable<ClientAccountInclude> AccountIncludes { get; set; }

		public ClientAddAccount GetValue()
		{
			return new ClientAddAccount
			{
				SpendTypeId = SpendTypeId,
				CurrencyId = CurrencyId,
				AccountName = AccountName,
				AccountGroupId = AccountGroupId,
				FinancialEntityId = FinancialEntityId,
				PeriodDefinitionId = PeriodDefinitionId,
				AccountTypeId = AccountTypeId,
				BaseBudget = BaseBudget.GetValue(),
				HeaderColor = HeaderColor,
				AccountIncludes = AccountIncludes
			};
		}
	}

	public class MvcClientEditAccount
	{
		public int AccountId { get; set; }
		public string AccountName { get; set; }
		public MvcNumber BaseBudget { get; set; }
		public FrontStyleData HeaderColor { get; set; }
		public FullAccountInfoViewModel.AccountType AccountTypeId { get; set; }
		public int? SpendTypeId { get; set; }
		public int? FinancialEntityId { get; set; }
		public int AccountGroupId { get; set; }
		public IEnumerable<ClientAccountInclude> AccountIncludes { get; set; }
		public IEnumerable<AccountFiedlds> EditAccountFields { get; set; }

		public ClientEditAccount GetValue()
		{
		    return new ClientEditAccount
		    {
		        SpendTypeId = SpendTypeId,
		        AccountId = AccountId,
		        AccountName = AccountName,
		        AccountGroupId = AccountGroupId,
		        FinancialEntityId = FinancialEntityId,
		        AccountTypeId = AccountTypeId,
		        BaseBudget = BaseBudget?.GetValue(),
		        HeaderColor = HeaderColor,
		        AccountIncludes = AccountIncludes,
		        EditAccountFields = EditAccountFields
		    };
		}
	}

    public class BaseModel
    {
        public MainHeaderModel HeaderModel { get; set; }
        public IHtmlHeaderHelper HtmlHeaderHelper { get; set; }
    }

    public class AddTransferModel
    {
        public int AccountPeriodId { get; set; }
        public int DestinationAccountId { get; set; }
        public float Amount { get; set; }
        public int AmountCurrencyId { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public JsDateTime TransferDateTime { get; set; }
        public int SpendTypeId { get; set; }
        public bool IsPending { get; set; }
        public string Description { get; set; }
    }

    public class AddSpendUpdateAccountIncludeModel
    {
        public int AccountId { get; set; }
        public int CurrencyId { get; set; }
        public IEnumerable<AddSpendAccountDataModel> IncludedAccounts { get; set; }
    }

	public class AddBasicTrxDataModel
	{
		public float Amount { get; set; }
		public JsDateTime SpendDate { get; set; }
		public int SpendTypeId { get; set; }
		public string Description { get; set; }
		public int CurrencyId { get; set; }
		public bool IsPending { get; set; }
	}

	public class AddBasicTrxByPeriod : AddBasicTrxDataModel
	{
		public int AccountPeriodId { get; set; }
	}

	public class AddSpendDataModel : AddBasicTrxDataModel
    {
        public AddSpendAccountDataModel OriginalAccountData { get; set; }
        public IEnumerable<AddSpendAccountDataModel> IncludedAccounts { get; set; }
    }

    public class AddSpendAccountDataModel
    {
        public int AccountId { get; set; }
        public int ConvertionMethodId { get; set; }
    }

    public class JsDateTime : BaseModel
    {
        #region Properties 

        public int Year { set; get; }
        public int Month { set; get; }
        public int Day { set; get; }
        public int Hours { set; get; }
        public int Minutes { set; get; }
        public int Seconds { set; get; }

        #endregion

        #region Methods

        public DateTime GetDate()
        {
            return new DateTime(Year, Month, Day, Hours, Minutes, Seconds);
        }

        #endregion
    }

    internal class CustomDate : BaseModel
    {
        public CustomDate(){}

        public CustomDate(int year, int month, int day, int hours, int minutes, int seconds)
        {
            Year = year;
            Month = month;
            Day = day;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public bool Equals(CustomDate customDate, bool compareSeconds)
        {
            if (customDate == null)
                return false;
            return customDate.Year == Year && customDate.Month == Month && customDate.Day == Day &&
                   customDate.Hours == Hours && customDate.Minutes == Minutes &&
                   (!compareSeconds || customDate.Seconds == Seconds);
        }

        #region Properties

        public int Year { set; get; }
        public int Month { set; get; }
        public int Day { set; get; }
        public int Hours { set; get; }
        public int Minutes { set; get; }
        public int Seconds { set; get; }

        #endregion

        private class InternalCustomDate
        {
#pragma warning disable 649
            // ReSharper disable InconsistentNaming
            public int _year;
            public int _month;
            public int _day;
            public int _hours;
            public int _minutes;
            public int _seconds;
            // ReSharper restore InconsistentNaming
#pragma warning restore 649
        }

        #region Methods

        public DateTime GetDateTime()
        {
            return new DateTime(Year, Month, Day, Hours, Minutes, Seconds);
        }

        #endregion

        #region Statics Methods

        public static CustomDate CreateCustomDate(string dateString)
        {
            InternalCustomDate internalCustomDate = (new JavaScriptSerializer().Deserialize<InternalCustomDate>(dateString));
            CustomDate customDate = new CustomDate(internalCustomDate._year, internalCustomDate._month,
                                                   internalCustomDate._day, internalCustomDate._hours,
                                                   internalCustomDate._minutes, internalCustomDate._seconds);
            return customDate;
        }

        #endregion
    }

    internal class BasicSpend : BaseModel
    {
        public int Id;
        public string userId;
        public DateTime SpendDate;
        public float Amount;
        public string AccountIds;
        public string Description;
        public int SpendTypeId;
        public string CustomDateString;

        public DateTime GetDate()
        {
            CustomDate customDate = GetCustomDate();
            return customDate.GetDateTime();
        }

        public CustomDate GetCustomDate()
        {
            return CustomDate.CreateCustomDate(CustomDateString); 
        }

        public static BasicSpend CreateBasicSpend(string jsonObject)
        {
            if (string.IsNullOrEmpty(jsonObject))
                return new BasicSpend();
            BasicSpend basicSpend = (new JavaScriptSerializer().Deserialize<BasicSpend>(jsonObject));
            return basicSpend;
        }
    }

    public class LoginModel : BaseModel
    {
        #region Properties

        [Display(Name = "Username")]
        [Required]
        public string Username { set; get; }

        [Display(Name = "Password")]
        [Required]
        public string Password { set; get; }

        [Display(Name = "Remember me")]
        public bool RememberMe { set; get; }

        #endregion
    }

    internal class MainModel : BaseModel
    {
        #region Constructor

        public MainModel()
        {
            EventDate = DateTime.Now;
        }

        #endregion

        [Range(1, double.MaxValue, ErrorMessage = "Please enter valid number")]
        [Display(Name = "Amount")]
        [Required]
        public double Amount { set; get; }

        [Display(Name = "Type")]
        [Required]
        public int SpendTypeId { set; get; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        [Required]
        public DateTime EventDate { set; get; }

        public IEnumerable<SpendType> SpendTypes { set; get; }

        public double CurrentWeeklyAmount { set; get; }

        public string DetailTable { set; get; }
    }

    public class LoanPageModel : BaseModel
    {
        public bool IsEmpty { get; set; }
        public LoanQueryCriteria CriteriaId { get; set; } = LoanQueryCriteria.UserId;
        public IEnumerable<BasicLoanAccountModel> Acccounts { get; set; } = new BasicLoanAccountModel[0];
    }

    public class AddLoanPaymentModel
    {
        public float Amount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsPending { get; set; }
        public string Description { get; set; }
        public int LoanRecordId { get; set; }
		public bool FullPayment { get; set; }
	}

    public class AddLoanModel : ClientLoanViewModel
    {
        public bool SameAsSource { get; set; }
    }

    public class MainSpendTypePageModel : BaseModel
    {
        
    }

	public class AccountDetailsViewPageModel : BaseModel
	{
        public AccountMainViewModel Model { get; set; }
	}

	public class MainViewPageModel : BaseModel
    {
        public UserAccountsViewModel Model { get; set; }
    }

    public class BasicLoanAccountModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public IEnumerable<LoanReportViewModel> Loans { get; set; }
    }
}