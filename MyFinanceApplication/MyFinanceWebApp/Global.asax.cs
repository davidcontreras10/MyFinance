using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyFinanceWebApp.Autofac;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;
using MyFinanceWebApp.Services.WebApiServices;

namespace MyFinanceWebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RegisterAutoFac(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void RegisterAutoFac(HttpConfiguration config)
        {
            // Setup the Container Builder
            var builder = new ContainerBuilder();

            // Register the controller in scope 
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register types
            RegisterTypes(builder);

            // Build the container
            var container = builder.Build();
			builder.RegisterFilterProvider();

            // Setup the dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutoFacContainer(new AutofacDependencyResolver(container));

			// Setup global filters
	        GlobalFilters.Filters.Add(new TokenAuthorizeAttribute());
			GlobalFilters.Filters.Add(new HandleTokenErrorAttribute());
        }

	    private static void RegisterTypes(ContainerBuilder builder)
	    {
		    if (builder == null)
		    {
			    throw new ArgumentNullException(nameof(builder));
		    }

		    builder.RegisterType<WebApiSpendService>().As<ISpendService>();
		    builder.RegisterType<WebApiUserService>().As<IUserService>();
		    builder.RegisterType<WebApiTransferService>().As<ITransferService>();
		    builder.RegisterType<WebApiAccountService>().As<IAccountService>();
		    builder.RegisterType<WebApiAccountGroupService>().As<IAccountGroupService>();
		    builder.RegisterType<BootstrapHtmlHeaderHelper>().As<IHtmlHeaderHelper>();
	        builder.RegisterType<WebApiSpendTypeService>().As<ISpendTypeService>();
            builder.RegisterType<WebApiLoanService>().As<ILoanService>();
            builder.RegisterType<WebApiScheduledTasksService>().As<IScheduledTasksService>();
            builder.RegisterType<WebApiExecutedTasksService>().As<IExecutedTasksService>();
            builder.Register<IHttpClientFactory>(_ =>
            {
	            var services = new ServiceCollection();
	            services.AddHttpClient();
	            var provider = services.BuildServiceProvider();
	            return provider.GetRequiredService<IHttpClientFactory>();
            });

            builder.RegisterType<WebApiUserService>().As<IUserService>().InstancePerRequest();
			builder.RegisterType<TokenAuthorizeAttribute>().PropertiesAutowired();
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>();

	    }
    }
}