using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Domain.Services;
using DataAccess;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            ConfigureAutofac(config);
        }

        private static void ConfigureAutofac(HttpConfiguration config)
        {
	        var builder = new ContainerBuilder();
	        builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			RegisterTypes(builder);
			var container = builder.Build();
			var resolver = new AutofacWebApiDependencyResolver(container);
			config.DependencyResolver = resolver;
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
	        builder.RegisterType<CurrencyServiceConnectionConfig>().As<IConnectionConfig>();
	        builder.RegisterType<ExchangeCurrencyDataService>().As<IExchangeCurrencyDataService>();
	        builder.RegisterType<DolarColonesBccrService>().As<IDolarColonesBccrService>();
	        builder.RegisterType<BccrCurrencyService>().As<IBccrCurrencyService>();
	        builder.RegisterType<BccrWebApiService>().As<IBccrCurrencyRepository>();

			builder.Register<IHttpClientFactory>(_ =>
			{
				var services = new ServiceCollection();
				services.AddHttpClient();
				var provider = services.BuildServiceProvider();
				return provider.GetRequiredService<IHttpClientFactory>();
			});
		}
    }
}
