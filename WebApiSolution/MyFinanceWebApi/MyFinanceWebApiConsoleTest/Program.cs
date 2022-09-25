using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceModel.WebMethodsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace MyFinanceWebApiConsoleTest
{
	class Program
    {
        static void Main(string[] args)
        {
			ByteTest();
            Console.ReadLine();
        }

		private static void ByteTest()
		{
			var b = new byte();
			b = 255;
			Console.WriteLine(b) ;
		}

		private static void CompareResultTest()
		{
		
			int o = 4;
			var c = "4";
			CompareResult(o, c);
			//Console.WriteLine(result);
		}

		private static void CompareResult(object o, string c)
		{
			
			object parseC = int.Parse(c);
			var result = Equals(o, parseC);
			Console.WriteLine(result);
		}

		private static void RegexTests()
		{
			var regex = new Regex(@"^(?<init_path>[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*)(?<extension>=>)(?<predicate>[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*)(?<operator>==|=>|=<|<>|<|>)(?<value>.+)$");
			var input = "mainmodel.accounts=>details.id==1";
			var match = regex.Match(input);
			var initPath = match.Groups["init_path"];
			var extension = match.Groups["extension"];
			var predicate = match.Groups["predicate"];
			var operation = match.Groups["operator"];
			var value = match.Groups["value"];
			var replaced = regex.Replace(input, "${init_path}");
			Console.ReadKey();
		}

		private static void RegexSplitTest()
		{
			var splitted = Regex.Split("(53),(32),(52)", @"^\(.*\)$", RegexOptions.Compiled);

		}

		private static void SplitTest()	
		{
			var str = "";
			var array = str.Split(',');

		}

        private static int counter;
        public static string CreateRandomString(int length)
        {
            length -= 12; //12 digits are the counter
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            long count = System.Threading.Interlocked.Increment(ref counter);
            byte[] randomBytes = new byte[length * 3 / 4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            byte[] buf = new byte[8];
            buf[0] = (byte)count;
            buf[1] = (byte)(count >> 8);
            buf[2] = (byte)(count >> 16);
            buf[3] = (byte)(count >> 24);
            buf[4] = (byte)(count >> 32);
            buf[5] = (byte)(count >> 40);
            buf[6] = (byte)(count >> 48);
            buf[7] = (byte)(count >> 56);
            return Convert.ToBase64String(buf) + Convert.ToBase64String(randomBytes);
        }

        public static void GetQueryStringTest()
        {
            var value = new CustomModel
            {
                Date = DateTime.Now,
                Name = "david",
                Id = "1"
            };
            var queryString = GetQueryString(value, "customModel");
        }

        public static string GetQueryString(object obj, string paramName)
        {
            var properties = from p in obj.GetType().GetProperties()
                where p.GetValue(obj, null) != null
                select paramName + "." + p.Name + "=" + HttpUtility.HtmlEncode(p.GetValue(obj, null).ToString());

            return string.Join("&", properties.ToArray());
        }

        public static void CreateUserToken()
        {
            string url = "Test/GetToken?userId=test";
            string tokenResult = HttpClientUtilities.GetResponseType<string>(url, false, null);

            string url2 = "Test/SomeAction?userId=test&tokenId=" + tokenResult;
            HttpResponseMessage response = HttpClientUtilities.GetResponse(url2, false, null);
        }

        public static void SimpleTest()
        {
            string url = "Test/SomeAction?userId=test&tokenId=asdfa561s6df56sd1f";
            HttpResponseMessage response = HttpClientUtilities.GetResponse(url, false, null);
        }

        public static void TestGetSpendTypes()
        {
            string url = "Spends/GetSpendTypes";
            List<SpendType> spendTypes = HttpClientUtilities.GetResponseType<List<SpendType>>(url, false, null);
        }

        public static void TestAddSpendByAccount()
        {
            AddSpendByAccountModel model = new AddSpendByAccountModel()
            {
                AccountIds = "1,2",
                Amount = 5303,
                Date = DateTime.Now,
                SpendType = 1,
                UserId = "test"
            };
            string url = "Spends/AddSpendByAccount";
            HttpResponseMessage response = HttpClientUtilities.GetResponse(url, true, model);
        }

        //_______________

        private static void TestSpendGetDateRange()
        {
            GetDateRangeModel model = new GetDateRangeModel
            {
                AccountIds = "1,2",
                Date = DateTime.Now,
                DateSpecified = true,
                UserId = "test"
            };
            DateRange dateRange = HttpClientUtilities.GetResponseType<DateRange>("Spends/GetDateRange", true, model);
        }

        
        public static void TestGetAddPeriodData()
        {
            string url = "Spends/GetAddPeriodData?username=test&accountId=1";
            AddPeriodData addPeriodData = HttpClientUtilities.GetResponseType<AddPeriodData>(url, false, null);
        }

        public static void TestAddPeriod()
        {
            AddPeriodModel model = new AddPeriodModel
            {
                AccountId = 1,
                Budget = 5000,
                End = DateTime.Now.AddDays(7),
                Initial = DateTime.Now,
                UserId = "test"
            };
            string url = "Spends/AddPeriod";
            HttpResponseMessage response = HttpClientUtilities.GetResponse(url, true, model);
        }

        public static void TestDeleteSpend()
        {
            string url = "Spends/DeleteSpend?username=test&spendId=1";
            HttpResponseMessage response = HttpClientUtilities.GetResponse(url, true, null);
        }

        public static void TestGetSpendsInfo()
        {
            GetSpendsInfoModel model = new GetSpendsInfoModel
            {
                UserId = "test"
            };
            string url = "Spends/GetSpendsInfo";
            List<Spend> listSpend = HttpClientUtilities.GetResponseType<List<Spend>>(url, true, model);
        }
       

        //public static void DateTimeTest()
        //{
        //    DateTime datetimeValue = DateTime.Now;
        //    string url = "Spends/DateTimeTest?date=" + datetimeValue.ToString("s", CultureInfo.InvariantCulture);
        //    HttpResponseMessage response = HttpClientUtilities.GetResponse(url, false, null);
        //}

    }

    public class CustomModel
    {
        public string Id { get; set; }

        public DateTime? Date { get; set; }

        public string Name { get; set; }
    }
}
