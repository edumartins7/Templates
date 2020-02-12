using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace NetCore3Console
{
    class Program
    {
        private static Opcoes _opcoes;

        static void Main(string[] args)
        {
            ConfigurarLogs();
            SetGlobalCultureInfo();
            _opcoes = LerOpcoes();

            
        }

        static Opcoes LerOpcoes()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var opcoes = new Opcoes()
            {
                Teste = configuration.GetValue<string>("Teste")
            };

            return opcoes;
        }

        static void ConfigurarLogs()
        {

#if DEBUG
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
#if DEBUG
                .WriteTo.ColoredConsole(
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{NewLine}{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
                )
#endif


                .WriteTo.File(
                    path: "logs\\serilog.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 31
                )
                .CreateLogger();
        }

        static void SetGlobalCultureInfo()
        {
            var cultureInfo = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }
    }
}
