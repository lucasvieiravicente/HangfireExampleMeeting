using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace ExemploMeetingHangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigurarLogger();

            try
            {
                Log.Information("Iniciando o projeto.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Aplicação falhou ao iniciar");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void ConfigurarLogger()
        {
            Log.Logger = new LoggerConfiguration()
                                            .WriteTo.Console()
                                            .WriteTo.Debug()
                                            .Enrich.FromLogContext()
                                            .CreateLogger();
        }
    }
}
