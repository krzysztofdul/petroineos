using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Petroineos.Intraday
{
    public class CsvFileService : IFileService
    {
        private readonly ILogger<CsvFileService> _logger;
        private readonly ICsvSerializer _serializer;
        private readonly IConfigurationService _configuration;

        public CsvFileService(
            ILogger<CsvFileService> logger,
            ICsvSerializer serializer,
            IConfigurationService configuration)
        {
            _logger = logger;
            _serializer = serializer;
            _configuration = configuration;
        }

        public async void SaveToFileAsync(string fileName, IEnumerable<(string, decimal)> rows)
        {
            var reportsLocation = _configuration.ReportsLocation;

            if (!Directory.Exists(reportsLocation))
                Directory.CreateDirectory(reportsLocation);

            using (var stream = new StreamWriter(Path.Combine(reportsLocation, fileName)))
            {
                // header
                if (rows.Any())
                    await stream.WriteLineAsync(_serializer.GetHeader());
                
                // data
                foreach (var (time, volume) in rows)
                    await stream.WriteLineAsync(_serializer.GetRow(time, volume));
            }
        }

        public void WriteEmptySafe(string fileName)
        {
            try
            {
                SaveToFileAsync(fileName, Enumerable.Empty<(string, decimal)>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot write to {fileName}");
            }
        }
    }
}