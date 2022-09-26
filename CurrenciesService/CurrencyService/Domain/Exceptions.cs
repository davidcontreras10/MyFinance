using System;

namespace Domain
{
    public class InvalidExchangeRateCreationException : Exception
    {
        public InvalidExchangeRateCreationException(string message)
        {
            _message = message;
        }

        private readonly string _message;

        public override string Message
        {
            get
            {
                return _message;
            }
        }
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

        public override string Message
        {
            get
            {
                return "Invalid User\n" + base.Message;
            }
        }
    }

    public class InvalidBankUrlException : Exception
    {
        public InvalidBankUrlException(string url, string message)
        {
            _message = message;
            Url = url;
        }

        private readonly string _message;

        public override string Message
        {
            get { return _message; }
        }

        public string Url { get; private set; }
    }

    public class BccrWebServiceEntityNotFoundException : Exception
    {
        public BccrWebServiceEntityNotFoundException(string entityName) : base("No record found for " + entityName)
        {
        }
    }
}