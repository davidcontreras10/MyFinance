using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestGetBccrVentanillaModel();
            Console.ReadLine();
        }

        //private static void TestGetBccrVentanillaModel()
        //{
        //    var service = new ExchangeCurrencyDataService(new CurrencyServiceConnectionConfig());
        //    while (true)
        //    {
        //        try
        //        {
        //            var entityName = Console.ReadLine();
        //            var initial = DateTime.Now;
        //            var response = service.GetBccrVentanillaModel(entityName, DateTime.Today);
        //            var end = DateTime.Now;
        //            Console.WriteLine(string.Format("Init: {0} -- End: {1}", initial.ToString("hh.mm.ss.ffffff"), end.ToString("hh.mm.ss.ffffff")));
        //            if (response.Any())
        //            {
        //                var last = response.OrderByDescending(item => item.LastUpdate).ElementAt(0);
        //                Console.WriteLine(string.Format("Sell: {0} -- Purchase: {1}", last.Sell, last.Purchase));
        //            }
        //        }
        //        catch(BccrWebServiceEntityNotFoundException)
        //        {

        //        }
        //    }
        //}
        

        private class DateTimeContainer
        {
            public DateTime Value { set; get; }
        }

    }
}
