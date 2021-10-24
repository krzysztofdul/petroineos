using System;
using System.Collections.Generic;
using System.Linq;
using Services;

namespace Petroineos.Intraday
{
    public interface IAggregatorServise
    {
        IEnumerable<(int, decimal)> AggregateTrades(IEnumerable<PowerTrade> trades);
    }

    public class TradeAggregatorService : IAggregatorServise
    {
        /// <summary>
        /// Expects trades with 24 periods each
        /// </summary>
        public IEnumerable<(int, decimal)> AggregateTrades(IEnumerable<PowerTrade> trades)
        {
            var numberOfPeriods = 24;
            var count = trades.Count();
            if (count == 0)
                return Array.Empty<(int, decimal)>();

            return Enumerable.Range(1, numberOfPeriods).Select(period =>
            {
                return (
                    period,
                    Convert.ToDecimal(trades.Sum(t => t.Periods[period - 1].Volume) / count));
            });
        }
    }
}