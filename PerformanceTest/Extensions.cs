using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTest
{
    public static class Extensions
    {
        public static double Median(this IEnumerable<double> values)
        {
            int count = values.Count();
            values = values.OrderBy(n => n);
            int midpoint = count / 2;
            return (values.ElementAt(midpoint - 1) + values.ElementAt(midpoint)) / 2D;
        }
    }
}
