using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Services;

namespace Petroineos.Intraday.UnitTests
{
    public class TradeAggregatorServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void NoTradesReturnEmptyResult()
        {
            var service = new TradeAggregatorService();
            var res = service.AggregateTrades(Enumerable.Empty<PowerTrade>());
            Assert.IsEmpty(res);
        }

        [Test]
        public void AggregationReturns24PeriodsWithAggregatedSum()
        {
            var service = new TradeAggregatorService();

            var powerTrade1 = CreatePowerTrade();
            var powerTrade2 = CreatePowerTrade();

            var powerTrades = new List<PowerTrade>
            {
                powerTrade1,
                powerTrade2
            };
            var res = service.AggregateTrades(powerTrades);

            var index = 0;
            foreach (var (period, volume) in res)
            {
                Assert.AreEqual(period, powerTrade1.Periods[index].Period);
                Assert.AreEqual(volume, Convert.ToDecimal(powerTrades.Sum(t => t.Periods[index].Volume) / powerTrades.Count));
                index++;
            }
            Assert.AreEqual(24, res.Count());
        }

        private static PowerTrade CreatePowerTrade()
        {
            var trade = PowerTrade.Create(DateTime.Now, 24);
            foreach (var period in trade.Periods)
            {
                period.Volume = new Random().NextDouble() * 1000.0;
            }

            return trade;
        }
    }
}