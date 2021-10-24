using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Petroineos.Intraday
{
    public interface IReportSchedulerService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }

    public class ReportSchedulerService : IHostedService, IReportSchedulerService
    {
        private readonly ILogger<ReportSchedulerService> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IFileService _fileService;
        private Timer _timer;

        public ReportSchedulerService(
            ILogger<ReportSchedulerService> logger,
            IConfigurationService configurationService,
            IReportGenerator reportGenerator,
            IFileService fileService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _reportGenerator = reportGenerator;
            _fileService = fileService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("****** START *******");

            // Schedule first run report
            await ScheduleReportFor(DateTime.Now);
            _timer = new Timer(ScheduleNextReport, null, _configurationService.ReportsFrequency, _configurationService.ReportsFrequency);
        }

        private async void ScheduleNextReport(object state)
        {
            await ScheduleReportFor(DateTime.Now);
        }

        public async Task ScheduleReportFor(DateTime reportRunTime, int retryCounter = 0)
        {
            var reportTimeString = $"{reportRunTime:yyyyMMdd_HHmm}";
            var fileName = string.Format($"PowerPosition_{reportTimeString}.csv");

            _logger.LogInformation($"Report {reportTimeString} START");

            IEnumerable<(string, decimal)> reportData;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                reportData = await _reportGenerator.GenerateAsync(reportRunTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Report {reportTimeString} ERROR - There was an exception while retrieving trades.");

                // Retry few times, write empty file when all attempts fail
                // TODO Move retry counts to config
                if (retryCounter == 10)
                {
                    // Write empty file 
                    _fileService.WriteEmptySafe(fileName);
                    return;
                }

                // TODO Move sleep to config
                Thread.Sleep(20);
                await ScheduleReportFor(reportRunTime, ++retryCounter);
                return;
            }

            try
            {
                _fileService.SaveToFileAsync(fileName, reportData);
                _logger.LogInformation($"Report {reportTimeString} DONE - took: {stopWatch.ElapsedMilliseconds}ms");

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Report {reportTimeString} ERROR - There was an exception while saving report file.");
                _fileService.WriteEmptySafe(fileName);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("****** STOP *******");
            _timer.Dispose();
            return Task.CompletedTask;
        }
    }
}