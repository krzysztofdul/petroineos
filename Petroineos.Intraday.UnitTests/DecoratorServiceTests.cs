using System;
using System.Linq;
using NUnit.Framework;

namespace Petroineos.Intraday.UnitTests
{
    public class DecoratorServiceTests
    {
        [Test]
        public void DecoratorServiceThrowsWhenUnexpectedNumberOfPeriods()
        {
            var service = new DecoratorService();
            Assert.Throws<ArgumentException>(() => service.DecoratePeriods(Enumerable.Empty<(int, decimal)>()));
        }

        [Test]
        public void DecoratePeriodsConvertsPeriodIdToHourString()
        {
            var periodInTime = new[]
            {
                "23:00", "00:00", "01:00", "02:00", "03:00", "04:00", 
                "05:00", "06:00", "07:00", "08:00", "09:00", "10:00",
                "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", 
                "17:00", "18:00", "19:00", "20:00", "21:00", "22:00"
            };

            var service = new DecoratorService();
            var res = service.DecoratePeriods(Enumerable.Range(1, 24).Select(i => (i, 0m)));

            var index = 0;

            foreach (var (time, _) in res)
            {
                Assert.AreEqual(periodInTime[index], time);
                index++;
            }
        }

    }
}