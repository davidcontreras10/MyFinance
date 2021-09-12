using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFinanceModel;

namespace MyFinanceWebApi.Tests
{
    [TestClass]
    public class UnitTestUser
    {
        [TestMethod]
        public void TestMethod1()
        {
            string url = "api/User/ResultLoginAttempt?username=test&password=a";
            LoginResult result = HttpClientUtilities.GetResponseType<LoginResult>(url, false, null);
            Assert.IsNotNull(result);
        }
    }
}
