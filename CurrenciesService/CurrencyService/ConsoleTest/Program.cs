using CurrencyService;
using CurrencyService.Services;
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

        private static void TestBccrWebService()
        {
            var service = new BccrWebService();
            bool seguir = true;
            while (seguir)
            {
                var date1 = DateTime.Now;
                var response2 = service.GetBccrSingleVentanillaModels("3215", DateTime.Today.AddDays(-1), DateTime.Today);
                var response1 = service.GetBccrSingleVentanillaModels("3181", DateTime.Today.AddDays(-1), DateTime.Today);
                var datesMatch = response1.ElementAt(0).LastUpdate == response2.ElementAt(0).LastUpdate;
                var date2 = DateTime.Now;
                Console.WriteLine(string.Format("Init: {0} -- End: {1}", date1.ToString("hh.mm.ss.ffffff"), date2.ToString("hh.mm.ss.ffffff")));
                var line = Console.ReadLine();
                if (line == "fin")
                    seguir = false;
            }
        }

        private static void TestIEnumerableSort()
        {
            var dateTimeContainer = new List<DateTimeContainer>();
            dateTimeContainer.Add(new DateTimeContainer { Value = DateTime.Now.AddDays(-5) });
            dateTimeContainer.Add(new DateTimeContainer { Value = DateTime.Now.AddDays(-3) });
            dateTimeContainer.Add(new DateTimeContainer { Value = DateTime.Now.AddDays(-6) });
            dateTimeContainer.Add(new DateTimeContainer { Value = DateTime.Now.AddDays(-2) });
            var orderedList = dateTimeContainer.OrderByDescending(item => item.Value);
            foreach(var value in orderedList)
            {
                Console.WriteLine(value.Value.ToShortDateString());
            }
        }

        private class DateTimeContainer
        {
            public DateTime Value { set; get; }
        }

    }
}
