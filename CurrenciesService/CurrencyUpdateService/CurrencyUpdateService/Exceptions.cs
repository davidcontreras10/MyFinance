namespace CurrencyUpdateService
{
    using System;

    namespace CurrencyService
    {
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

            public string Url { get; private set; }
        }
    }
}
