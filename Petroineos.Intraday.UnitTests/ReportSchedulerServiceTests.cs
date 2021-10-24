using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Petroineos.Intraday.UnitTests
{
    public class ReportSchedulerServiceTests
    {
        [Test]
        public void RetryWhenError()
        {
            var logger = new Mock<ILogger<ReportSchedulerService>>();
            var config = new Mock<IConfigurationService>();
            var generator = new Mock<IReportGenerator>();
            var fileService = new Mock<IFileService>();

            config.Setup(c => c.ReportsLocation).Returns("Reports");
            var now = DateTime.Now;
            generator.Setup(c => c.GenerateAsync(now)).Throws(new Exception());

            var reportTimeString = $"{now:yyyyMMdd_HHmm}";
            var fileName = string.Format($"PowerPosition_{reportTimeString}.csv");
            var file = Path.Combine(config.Object.ReportsLocation, fileName);
            if (File.Exists(file))
                File.Delete(file);

            var service = new ReportSchedulerService(logger.Object, config.Object, generator.Object, fileService.Object);

            var x = service.ScheduleReportFor(now);
            x.Wait();

            generator.Verify(g => g.GenerateAsync(now), Times.Exactly(11));
            fileService.Verify(g => g.WriteEmptySafe(fileName), Times.Exactly(1));
        }
        
        [Test]
        public void TradesAreSavedAfterGeneration()
        {
            var logger = new Mock<ILogger<ReportSchedulerService>>();
            var config = new Mock<IConfigurationService>();
            var generator = new Mock<IReportGenerator>();
            var fileService = new Mock<IFileService>();

            config.Setup(c => c.ReportsLocation).Returns("Reports");
            var now = DateTime.Now;
            var trades = Enumerable.Range(1, 10).Select(r => ($"{r}", (decimal)r));
            generator.Setup(c => c.GenerateAsync(now)).Returns(Task.FromResult(trades));

            var reportTimeString = $"{now:yyyyMMdd_HHmm}";
            var fileName = string.Format($"PowerPosition_{reportTimeString}.csv");
            var file = Path.Combine(config.Object.ReportsLocation, fileName);
            if (File.Exists(file))
                File.Delete(file);

            var service = new ReportSchedulerService(logger.Object, config.Object, generator.Object, fileService.Object);

            var x = service.ScheduleReportFor(now);
            x.Wait();

            generator.Verify(g => g.GenerateAsync(now), Times.Exactly(1));
            fileService.Verify(g => g.SaveToFileAsync(fileName, trades), Times.Exactly(1));
        }
    }
}