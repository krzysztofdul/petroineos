using System;
using System.Collections.Generic;
using System.Linq;

namespace Petroineos.Intraday
{
    public interface IDecoratorService
    {
        IEnumerable<(string, decimal)> DecoratePeriods(IEnumerable<(int, decimal)> toDecorate);
    }

    /// <summary>
    /// Converts period into time
    /// </summary>
    public class DecoratorService : IDecoratorService
    {
        public IEnumerable<(string, decimal)> DecoratePeriods(IEnumerable<(int, decimal)> toDecorate)
        {
            if (toDecorate.Count() != 24)
                throw new ArgumentException(nameof(toDecorate), $"Unexpected number of periods expected 24 got {toDecorate.Count()}.");

            var decorated = new (string, decimal)[24];
            var current = 0;
            foreach (var (period, volume) in toDecorate)
            {
                var decoratedPeriod = period - 2;
                if (decoratedPeriod < 0)
                    decoratedPeriod = 23;

                decorated[current++] = ($"{decoratedPeriod:00}:00", volume);
            }

            return decorated;
        }
    }
}