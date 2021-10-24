using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;

namespace Petroineos.Intraday.Cmd
{
    class Program
    {
        private static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return 
                Host.CreateDefaultBuilder(args)
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
                        .AddHostedService<ReportSchedulerService>()
                        .AddLogging())
                    .ConfigureLogging(logging =>
                    {
                        logging.AddLog4Net("log4net.config");
                    });
        }
    }
}
