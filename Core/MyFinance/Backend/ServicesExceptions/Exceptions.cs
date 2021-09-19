using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using MyFinance.MyFinanceModel;

namespace MyFinance.Backend.ServicesExceptions
{
	public class HttpException : Exception
	{
		public HttpException(HttpResponseMessage responseMessage)
		{
			StatusCode = responseMessage.StatusCode;
			Content = responseMessage.Content;
		}

		public HttpException(HttpStatusCode httpStatusCode)
		{
			StatusCode = httpStatusCode;
		}

		public HttpException(HttpStatusCode httpStatusCode, string message) : base(message)
		{
			StatusCode = httpStatusCode;
		}
        
		public HttpException(HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner)
		{
			StatusCode = httpStatusCode;
		}

		public HttpStatusCode StatusCode { get; }
		public HttpContent Content { get; set; }
	}

    public class InvalidAddSpendCurrencyException:Exception
    {
        public InvalidAddSpendCurrencyException(IEnumerable<AccountCurrencyConverterData> invalidAccountsData)
        {
            InvalidAccountData = invalidAccountsData;
        }

        #region Attributes

        public IEnumerable<AccountCurrencyConverterData> InvalidAccountData { get; private set; }

        public override string Message => GetInvalidAccountsMessage();

        #endregion

        #region Methods

        private string GetInvalidAccountsMessage()
        {
            return "There was a missmatch between the amount currency and the target currency";
        }

        #endregion
    }

    public class InvalidExchangeRateCreationException : Exception
    {
        public InvalidExchangeRateCreationException(string message)
        {
            Message = message;
        }

        public override string Message { get; }
    }

    public class InvalidExchangeRateCreationArgException : InvalidExchangeRateCreationException
    {

        public InvalidExchangeRateCreationArgException(string invalidArgument)
            : base("")
        {
            InvalidArgument = invalidArgument;
        }

        public InvalidExchangeRateCreationArgException(string invalidArgument, string message) :
            base(message)
        {
            InvalidArgument = invalidArgument;
        }


        public string InvalidArgument { set; get; }
    }

    public class InvalidUserException : Exception
    {

        public InvalidUserException(string message)
            : base(message)
        {
        }

        public override string Message => "Invalid User\n" + base.Message;
    }

    public class InvalidBankUrlException : Exception
    {
        public InvalidBankUrlException(string url, string message)
        {
            Message = message;
            Url = url;
        }

        public override string Message { get; }

        public string Url { get; }
    }

    public class InvalidAmountException : Exception
    {
        private const string DefaultMessage = "Unknown error";

        public InvalidAmountException() : base(DefaultMessage)
        {
        }

        public InvalidAmountException(string message) : base(message)
        {
            
        }
    }

    public class MissingSettingException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="MissingSettingException" /> class with the name of the parameter that causes this exception.</summary>
        /// <param name="paramName">The name of the parameter that caused the exception. </param>
        public MissingSettingException(string paramName)
            : base(GetMessage(paramName))
        {
        }

        private static string GetMessage(string paramName)
        {
            return $"App setting {paramName} is missing in the web.config file";
        }
    }

    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message) : base(message)
        {
            
        }

        public DataNotFoundException(){}
    }

    public class SpendNotPendingException : Exception
    {
        public SpendNotPendingException(int spendId) : base($"SpendId={spendId} is not pending spend.")
        {
            
        }
    }

    public class InvalidSpendAmountType : Exception
    {
        public InvalidSpendAmountType() : base("Invalid spend amount type")
        {

        }
    }

    public class NoPeriodInDateException : Exception
    {
        public NoPeriodInDateException(DateTime dateTime) : base($"No period existing for date: {dateTime}")
        {

        }
    }

    public class LoanPaymentException : Exception
    {
        public enum ErrorType
        {
            Unknown,
            AmountGreaterThanPending
        }

        public LoanPaymentException(ErrorType errorType) : base(GetErrorMessage(errorType))
        {
            Error = errorType;
        }

        private static string GetErrorMessage(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.Unknown: return "Unknown error";
                case ErrorType.AmountGreaterThanPending: return "Amount is greater than pending";
                default: return "Error type not implemented";
            }
        }

        public ErrorType Error { get; set; }
    }
}
