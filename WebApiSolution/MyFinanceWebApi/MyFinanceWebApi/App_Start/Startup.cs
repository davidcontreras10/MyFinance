using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using MyFinanceWebApi;
using Owin;
using MyFinanceWebApi.Authorization;

[assembly: OwinStartup(typeof(Startup))]

namespace MyFinanceWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            app.UseWebApi(config);

            app.UseAutofacMiddleware(WebApiConfig.Container);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureOAuth(app);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = AuthCommonSettings.AuthExpireToken,
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}
