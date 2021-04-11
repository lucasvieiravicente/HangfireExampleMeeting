using ExemploMeetingHangfire.Contexts;
using Hangfire;
using Hangfire.SqlServer;
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
            AdicionarHangfire(services, configuration);
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

        public static void UsarHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
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

        private static void AdicionarHangfire(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(hangfireConfig => hangfireConfig
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(configuration.GetConnectionString("Hangfire"),
                        new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true,
                            SchemaName = "Hangfire"
                        }));
        }
    }
}
