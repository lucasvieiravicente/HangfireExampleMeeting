using ExemploMeetingHangfire.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace ExemploMeetingHangfire.Configs
{
    public static class StartupConfig
    {
        public static void ConfigurarServicos(this IServiceCollection services, IConfiguration configuration)
        {
            AdicionarSwaggerGenInfo(services);
            AdicionarContextoEF(services, configuration);
            InjecaoDeDependencia.InjetarServicos(services);
            InjecaoDeDependencia.InjetarRepositorios(services);
        }

        public static void ConfigurarSwagger(this IApplicationBuilder app)
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
