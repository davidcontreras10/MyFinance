using DataAccess;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyServiceCore
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
			services.AddHttpClient();
			services.AddSingleton<IConfiguration>(Configuration);

			services.AddSwaggerGen();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Implement Swagger UI",
					Description = "A simple example to Implement Swagger UI",
				});
			});

			RegisterDependencies(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

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
		private static void RegisterDependencies(IServiceCollection services)
		{
			services.AddSingleton<IConnectionConfig, ConnectionStringConfig>();
			services.AddScoped<IExchangeCurrencyDataService, ExchangeCurrencyDataService>();
			services.AddScoped<IDolarColonesBccrService, DolarColonesBccrService>();
			services.AddScoped<IBccrCurrencyService, BccrCurrencyService>();
			services.AddScoped<IBccrCurrencyRepository, BccrWebApiService>();
		}
	}

}
