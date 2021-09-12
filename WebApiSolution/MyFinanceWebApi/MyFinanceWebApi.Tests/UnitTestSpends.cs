using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFinanceModel;
using MyFinanceModel.WebMethodsModel;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MyFinanceWebApi.Tests
{
    [TestClass]
    public class UnitTestSpends
    {
        [TestMethod]
        public void TestMethodEditSpend()
        {
            EditSpendModel model = new EditSpendModel
            {
                SpendId = 10,
                AccountIds = "AccountIds",
                Amount = 20,
                Description = "Description",
                ModifyList = "ModifyList",
                SpendDate = DateTime.Now,
                SpendTypeId = 30,
                Username = "Username"
            };
            HttpResponseMessage response = HttpClientUtilities.GetResponse("api/Spends/EditSpend", true, model);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void TestGetDatReange()
        {
            GetDateRangeModel model = new GetDateRangeModel
            {
                Username = "test",
                AccountIds = "1,2",
                Date = DateTime.Now,
                DateSpecified = 1
            };
            DateRange range = HttpClientUtilities.GetResponseType<DateRange>("api/Spends/GetDateRange", true, model);
            Assert.IsNotNull(range);
        }
    }
}
