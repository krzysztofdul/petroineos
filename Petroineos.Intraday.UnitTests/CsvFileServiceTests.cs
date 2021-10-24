using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Petroineos.Intraday.UnitTests
{
    public class CsvFileServiceTests
    {
        [Test]
        public void WriteEmptySafeSavesEmptyFile()
        {
            var logger = new Mock<ILogger<CsvFileService>>();
            var config = new Mock<IConfigurationService>();

            config.Setup(c => c.ReportsLocation).Returns("Reports");

            var service = new CsvFileService(logger.Object, new IntradayCsvSerializer(), config.Object);

            var fileName = "emptyFile.csv";
            var file = Path.Combine(config.Object.ReportsLocation, fileName);
            
            if (File.Exists(file))
                File.Delete(file);

            Assert.IsFalse(File.Exists(file));
            service.WriteEmptySafe(fileName);
            Assert.IsTrue(File.Exists(file));
        }

        [Test]
        public void SaveToFileAsyncCreatesFileWithData()
        {
            var logger = new Mock<ILogger<CsvFileService>>();
            var config = new Mock<IConfigurationService>();

            config.Setup(c => c.ReportsLocation).Returns("Reports");

            var serializer = new IntradayCsvSerializer();
            var service = new CsvFileService(logger.Object, serializer, config.Object);

            var fileName = "report.csv";
            var file = Path.Combine(config.Object.ReportsLocation, fileName);
            
            if (File.Exists(file))
                File.Delete(file);

            Assert.IsFalse(File.Exists(file));
            var values = Enumerable.Range(1, 24).Select(r => ($"{r}", (decimal)r));
            
            service.SaveToFileAsync(fileName, values);
            Assert.IsTrue(File.Exists(file));

            var fileLines = File.ReadAllLines(file);
            Assert.IsTrue(fileLines.Length == 25);

            Assert.AreEqual(serializer.GetHeader(), fileLines.First());

            var index = 1;
            foreach (var (time, value) in values)
            {
                Assert.AreEqual(serializer.GetRow(time, value), fileLines[index++]);
            }
        }
    }
}