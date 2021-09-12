using CurrencyUpdateService;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CurrencyExchangeLogicTest();
            Console.WriteLine();
        }

        private static void TestWebValuesInterface()
        {
            var service = new CurrencyExchangeUpdateService();
            service.Start();

        }

        private static void CurrencyExchangeLogicTest()
        {
            var service = new CurrencyExchangeLogic();
            service.UpdateCurrencyExchangeData();
        }


    }
}
