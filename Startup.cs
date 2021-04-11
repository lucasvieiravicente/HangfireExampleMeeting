using ExemploMeetingHangfire.Domains.Contexts;
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
using Microsoft.EntityFrameworkCore;

namespace ExemploMeetingHangfire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            AdicionarSwaggerGenInfo(services);
            AdicionarContextoEF(services, Configuration);
        }

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

            ConfigurarSwagger(app);
        }

        private static void ConfigurarSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HangfireMeeting API v1");
            });
        }

        private static void AdicionarSwaggerGenInfo(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HangfireMeeting API",
                    Description = "Api to exemplify the using of Hangfire",
                    Contact = new OpenApiContact
                    {
                        Name = "Lucas V Vicente",
                        Email = "lucasvieiravicente1@gmail.com",
                        Url = new Uri("https://white-moss-0cf7e1e0f.azurestaticapps.net/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        //Url = new Uri("inserir link da license do repositorio depois")
                    },
                });
            });
        }

        private static void AdicionarContextoEF(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EFContext>(x => x.UseSqlServer(configuration.GetConnectionString("Database")));
        }
    }
}
