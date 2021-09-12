using System.Web;
using System.Web.Http;

namespace MyFinanceWebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //Custom filters
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}