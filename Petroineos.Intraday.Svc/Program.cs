using System.IO;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;

namespace Petroineos.Intraday.Svc
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            // Set current directory to where the service exe is located
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            //Debugger.Launch();

            var container = CreateHostBuilder(args).Build();
            container.Start();
            var servicesToRun = new ServiceBase[]
            {
                new IntradayService(container.Services.GetService<IReportSchedulerService>())
            };
            ServiceBase.Run(servicesToRun);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return
                Host.CreateDefaultBuilder(args)
                    .ConfigureLogging(logging =>
                    {
                        logging.AddLog4Net("log4net.config");
                        logging.AddEventSourceLogger();
                        logging.AddEventLog(s => s.SourceName = "Petroineos.Intraday.Svc");
                    })
                    .ConfigureAppConfiguration(app =>
                    {
                        app.AddJsonFile("appsettings.json");
                    })
                    .ConfigureServices((_, services) =>
                        services
                            .AddSingleton<IConfigurationService, ConfigurationService>()
                            .AddSingleton<ICsvSerializer, IntradayCsvSerializer>()
                            .AddTransient<IFileService, CsvFileService>()
                            .AddTransient<IPowerService, PowerService>()
                            .AddTransient<IAggregatorServise, TradeAggregatorService>()
                            .AddTransient<IDecoratorService, DecoratorService>()
                            .AddTransient<IReportGenerator, ReportGenerator>()
                            .AddSingleton<IReportSchedulerService, ReportSchedulerService>()
                            .AddLogging());
        }
    }
}
