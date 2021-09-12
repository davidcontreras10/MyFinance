using System.Reflection;
using System.Web.Http;
using System.Web.OData.Extensions;
using System.Web.OData.Query;
using Autofac;
using Autofac.Integration.WebApi;
using DataAccess;
using MyFinanceBackend.Data;
using MyFinanceBackend.Services;
using MyFinanceBackend.Services.AuthServices;
using MyFinanceWebApi.CustomHandlers;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.Application;

namespace MyFinanceWebApi
{
    public static class WebApiConfig
    {
        public static IContainer Container { private set; get; }

        public static void Register(HttpConfiguration config)
        {
			SetJsonResponses(config);

            // Web API filters
            AddGlobalFilters(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.Routes.MapHttpRoute(
                name: "MVC Default",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Redirect root to Swagger UI
            config.Routes.MapHttpRoute(
                name: "Swagger UI",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "swagger/ui/index"));


            var builder = new ContainerBuilder();

            // Register Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //Register services
            RegisterTypes(builder);

            config.SetDefaultQuerySettings(new DefaultQuerySettings {EnableSelect = true, EnableFilter = true});

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            Container = container;
            //return container;
        }

        private static void AddGlobalFilters(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ErrorHandlerExceptionAttribute());
            config.Filters.Add(new ValidateModelStateAttribute());
            config.Filters.Add(new ResourceAuthorizationFilter());
            config.MessageHandlers.Add(new RestrictObjectHandler());
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<LocalConnectionConfig>().As<IConnectionConfig>();
            builder.RegisterType<TransferService>().As<ITransferService>();
            builder.RegisterType<UsersService>().As<IUsersService>();
            builder.RegisterType<SpendsService>().As<ISpendsService>();
            builder.RegisterType<AccountsPeriodsService>().As<IAccountsPeriodsService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>();
            builder.RegisterType<SpendTypeService>().As<ISpendTypeService>();
            builder.RegisterType<AccountGroupRepository>().As<IAccountGroupRepository>();
            builder.RegisterType<AccountGroupService>().As<IAccountGroupService>();
            builder.RegisterType<SpendTypeRepository>().As<ISpendTypeRepository>();
            builder.RegisterType<UserRepository>().As<IUserRespository>();
            builder.RegisterType<SpendsRepository>().As<ISpendsRepository>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>();
            builder.RegisterType<UserAuthorizeService>().As<IUserAuthorizeService>();
            builder.RegisterType<AuthorizationDataRepository>().As<IAuthorizationDataRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<TransferRepository>().As<ITransferRepository>();

	        builder.RegisterType<LoanRepository>().As<ILoanRepository>();
			builder.RegisterType<ResourceAccessRepository>().As<IResourceAccessRepository>();

	        builder.RegisterType<AccountFinanceService>().As<IAccountFinanceService>();
	        builder.RegisterType<LoanService>().As<ILoanService>();
            SerilogSetup(builder);
		}

	    private static void SetJsonResponses(HttpConfiguration config)
	    {
		    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		    config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
		}

        private static void SerilogSetup(ContainerBuilder builder)
        {
            builder.Register<ILogger>((c, p) =>
            {
                return new LoggerConfiguration().ReadFrom.AppSettings()
                  .CreateLogger();
            }).SingleInstance();
        }
    }
}
