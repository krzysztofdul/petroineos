using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Services;

namespace Petroineos.Intraday
{
    public interface IReportGenerator
    {
        Task<IEnumerable<(string, decimal)>> GenerateAsync(DateTime reportRunTime);
    }

    public class ReportGenerator : IReportGenerator
    {
        private readonly ILogger<ReportGenerator> _logger;
        private readonly IPowerService _powerService;
        private readonly IAggregatorServise _aggregator;
        private readonly IDecoratorService _decorator;

        public ReportGenerator(
            ILogger<ReportGenerator> logger,
            IPowerService powerService,
            IAggregatorServise aggregator,
            IDecoratorService decorator)
        {
            _logger = logger;
            _powerService = powerService;
            _aggregator = aggregator;
            _decorator = decorator;
        }

        public async Task<IEnumerable<(string, decimal)>> GenerateAsync(DateTime reportRunTime)
        {
            // run current report and schedule feature jobs
            var trades = await _powerService.GetTradesAsync(reportRunTime);

            // TODO Add Trade Validator to validate number of periods and order
            var aggregated = _aggregator.AggregateTrades(trades);
            return _decorator.DecoratePeriods(aggregated);
        }
    }
}