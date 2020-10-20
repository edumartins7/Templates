using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace NetCore3Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigurarLogging();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
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


        public static void ConfigurarLogging()
        {
            // serve para debugar o próprio Serilog, manter comentado
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
#if DEBUG
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#else
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: "logs/serilog.log",
                    rollingInterval: RollingInterval.Day
                )
                .WriteTo.Logger(lc => lc.MinimumLevel.Information().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                     .Enrich.FromLogContext() //sub-logger para poder usar o FilterBy especifico para essa tabela de logs
                )
                .CreateLogger();

        }
    }
}