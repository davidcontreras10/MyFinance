using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using MyFinanceBackend.Services;
using MyFinanceModel;
using MyFinanceModel.Utilities;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

// ReSharper disable ConvertToConstant.Local
namespace ConsoleTest
{
    class Program
    {
        private static void Main(string[] args)
        {
	        DynamicCodeTest();
            Console.ReadKey();
        }

	    private static void DynamicCodeTest()
	    {
		    var appusers = new[]
		    {
			    new AppUser
			    {
				    Name = "user1",
				    UserId = Guid.NewGuid()
			    },
			    new AppUser
			    {
				    Name = "user2",
				    UserId = Guid.NewGuid()
			    },
			    new AppUser
			    {
				    Name = "user3",
				    UserId = Guid.NewGuid()
			    },
			    new AppUser
			    {

				    Name = "user4",
				    UserId = Guid.NewGuid()
			    },
		    };

		    const string lambdaQuery = "u => u.Name == \"user1\"";

		    var options = ScriptOptions.Default.AddReferences(typeof(AppUser).Assembly);
		    var lambdaExpression = CSharpScript.EvaluateAsync<Func<AppUser, bool>>(lambdaQuery, options).Result;
		    var result = appusers.Where(lambdaExpression);
	    }

        private static void TestBoolConvert()
        {
            int value = 1;
            bool result;
            result = bool.TryParse(value.ToString(), out result) && result;
            
        }

        private static void ToUrlComplexObjectTest()
        {
        }

        private static async Task<bool> EmailServiceTest()
        {
            var emailService = new EmailService();
            await emailService.SendEmailAsync("dcontre10@gmail.com", "Test1", "some body");
            return true;
        }

        private static void JsonParseTest()
        {
            var json = "{'headedfrColor': 'blue','borderColor': 'green'}";
            var style = ServicesUtils.CreateFrontStyleData(json);
        }

        private static void TestNumFormat()
        {
            float num = 255.315F;
            string value = $"{num:0.##}";
            Console.WriteLine(value);
        }

        static void AddSpendSpTest()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("AccountId");
            dataTable.Columns.Add("Purchase");
            dataTable.Columns.Add("Sell");
            dataTable.Rows.Add(1, 1, 1);
            dataTable.Rows.Add(2, 543, 555);
            var username = "test";

            var spendType = 1;
            var amount = 3000;
            var dateTime = DateTime.Now.AddDays(-2);
            var currency = 1;
            var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@pUsername", username),
                    new SqlParameter("@pSpendType", spendType),
                    new SqlParameter("@pAmount", amount),
                    new SqlParameter("@pDate", dateTime),
                    new SqlParameter("@pCurrencyId", currency),
                    new SqlParameter("@pAccountsTable", dataTable)
                };
            var storedProcedure = "SpSpendByAccountsAdd";
            var connectionString = ConfigurationManager.ConnectionStrings["SqlServerLocalConnection"].ConnectionString;
            var dataSet = ExecuteStoredProcedure(connectionString, parameters, storedProcedure);
        }

        private static DataSet ExecuteStoredProcedure(string connectionString, IEnumerable<SqlParameter> parameters,
                                                   string storedProcedure)
        {
            string connString = connectionString;
            var sqlConnection = new SqlConnection(connString);
            var cmd = new SqlCommand(storedProcedure, sqlConnection) { CommandType = CommandType.StoredProcedure };
            foreach (var sqlParameter in parameters)
            {
                cmd.Parameters.Add(sqlParameter);
            }
            var dataSet = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter(cmd);
            sqlDataAdapter.Fill(dataSet);
            return dataSet;
        }

        static void SqlTableParameterTypeTest()
        {
            var dataTable = new DataTable();
            //dataTable.Columns.Add("varInt1");
            //dataTable.Columns.Add("varInt2");
            //dataTable.Columns.Add("varValue1");
            dataTable.Columns.Add("");
            dataTable.Columns.Add("");
            dataTable.Columns.Add("");
            dataTable.Rows.Add(3, 4, "34");
            dataTable.Rows.Add(5, 6, "56");
            string connString = ConfigurationManager.ConnectionStrings["SqlServerTestConnection"].ConnectionString;
            var sqlConnection = new SqlConnection(connString);
            var cmd = new SqlCommand("dbo.TestListParameter", sqlConnection) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.AddWithValue("@pListparam", dataTable);
            var dataSet = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter(cmd);
            sqlDataAdapter.Fill(dataSet);

        }

        public static void DateTimeParseExactTest()
        {
            string stringDate = "02/06/2016    08:26 am.";//"02-06-2016 08:26 am"; 02/06/2016    08:26 am.
            string formatDate = "dd/MM/yyyy    hh:mm tt.";//"dd-MM-YYYY hh:mm tt"; dd/MM/yyyy    hh:mm tt.
            DateTime dateTime = DateTime.ParseExact(stringDate, formatDate, CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None);
        }



        public static void TestExchangeRateModel()
        {
            //var exachangeRate = CurrencyConverterService.GetTodaysExchangeRate();
        }

        private static void TestFixEndDate()
        {
            DateTime today = DateTime.Today;
            DateTime fixedDate = FixEndDate(today);
            Console.WriteLine(fixedDate);
        }

        public static DateTime FixEndDate(DateTime endDate)
        {
            DateTime dateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day);
            dateTime = dateTime.AddDays(1).AddSeconds(-1);
            return dateTime;
        }


        private static void WebServiceCallerTest()
        {
            WebServiceCaller caller = new WebServiceCaller("http://localhost:55396/MyFinanceSpendWebService.asmx", "GetSpendsInfo");
            caller.Params.Add("username", "dcontre10");
            string endDateString = DateTime.Now.ToString("s");
            endDateString = "2015-05-21T23:17:55";
            caller.Params.Add("endTime", endDateString);
            caller.Invoke(false);
            string resultObject = caller.GetResponseXml();
            List<Spend> spends = XmlConvertion.DeserializeToXml<List<Spend>>(resultObject);

        }

        private static void WeekDaysTest()
        {
            DateTime start = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            DateTime end = DateTime.Now.EndOfWeek(DayOfWeek.Monday);
        }



        //public static void FirstUserWebServiceTest()
        //{
        //    MyFinanceUserWebServiceSoapClient service = new MyFinanceUserWebServiceSoapClient();
        //    string resultLoginAttempt = service.ResultLoginAttempt("dcontre10", "perr10");
        //    LoginResult loginResult = XmlConvertion.DeserializeToXml<LoginResult>(resultLoginAttempt);
        //}

        private static void DbConnectionTest()
        {
            //SqlServerBaseService service=new SqlServerBaseService();
            //Console.WriteLine(dataSet.Tables.Count);    
        }

        private static void BackendTest()
        {
			//UsersService usersService = new UsersService(new LocalConnectionConfig());
			//AppUser user = usersService.GetUser("dcontre10");
			//Console.WriteLine(user.Name);
        }


        private static void SpendsServiceTest()
        {
            //SpendsService spendsService = new SpendsService(new LocalConnectionConfig(), new CurrencyService());
            //spendsService.AddSpend("dcontre10", 3, DateTime.Now, 5000);
        }

        //private static void WebServiceTest()
        //{
        //    var service = new SpendsServiceSoapClient();
        //    var result = service.DefaultSpend();
        //    var xdoc = XDocument.Parse(result.ToString());
        //    Spend spend = XmlConvertion.DeserializeToXml<Spend>(xdoc);
            
        //}

        //private static void WebServiceTest2()
        //{
        //    var service = new SpendsServiceSoapClient();
        //    var result = service.DefaultSpendXml();
        //    var xdoc = XDocument.Parse(result);
        //    Spend spend = XmlConvertion.DeserializeToXml<Spend>(xdoc);
        //}


    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            DateTime dateTime = dt.StartOfWeek(startOfWeek);
            dateTime = dateTime.AddDays(7);
            return dateTime;
        }
    }

}
// ReSharper restore ConvertToConstant.Local