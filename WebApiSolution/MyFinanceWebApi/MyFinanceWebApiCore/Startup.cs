using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyFinanceBackend.Data;
using MyFinanceBackend.Services;
using MyFinanceBackend.Services.AuthServices;
using MyFinanceWebApiCore.Authentication;
using MyFinanceWebApiCore.Config;
using MyFinanceWebApiCore.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceWebApiCore
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			RegisterServices(services);
			services.AddHttpClient();
			services.ConfigureSettings(Configuration);
			services.AddSingleton(Log.Logger);
			services.AddSwaggerGen();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Implement Swagger UI",
					Description = "A simple example to Implement Swagger UI",
				});

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Auth token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							}
						},
						new string[]{}
					}
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware<AuthenticationMiddleware>();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showing API V1");
			});
		}

		private static void RegisterServices(IServiceCollection services)
		{
			services.AddScoped<IConnectionConfig, SqlServerConfig>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();

			services.AddScoped<ITransferService, TransferService>();
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<ISpendsService,SpendsService>();
			services.AddScoped<IAccountsPeriodsService, AccountsPeriodsService>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<ICurrencyService, CurrencyService>();
			services.AddScoped<ISpendTypeService, SpendTypeService>();
			services.AddScoped<IAccountGroupRepository, AccountGroupRepository>();
			services.AddScoped<IAccountGroupService, AccountGroupService>();
			services.AddScoped<ISpendTypeRepository,SpendTypeRepository>();
			services.AddScoped<IUserRespository, UserRepository>();
			services.AddScoped<ISpendsRepository, SpendsRepository>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IAuthorizationService, AuthorizationService>();
			services.AddScoped<IUserAuthorizeService, UserAuthorizeService>();
			services.AddScoped<IAuthorizationDataRepository, AuthorizationDataRepository>();
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<ITransferRepository, TransferRepository>();
			services.AddScoped<IAutomaticTaskRepository, AutomaticTaskRepository>();

			services.AddScoped<ILoanRepository, LoanRepository>();
			services.AddScoped<IResourceAccessRepository, ResourceAccessRepository>();
			services.AddScoped<IScheduledTasksService, ScheduledTasksService>();

			services.AddScoped<IAccountFinanceService, AccountFinanceService>();
			services.AddScoped<ILoanService, LoanService>();
		}
	}
}
