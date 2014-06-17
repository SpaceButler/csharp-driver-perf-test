using Cassandra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class ThroughputTest
    {
        private ISession _session;

        public ThroughputTest(ISession session)
        {
            _session = session;
        }

        public double SelectSystemTest(int iterations, int threads)
        {
            Action action = () =>
            {
                for (var i = 0; i < iterations; i++)
                {
                    _session.Execute("SELECT * FROM system.schema_columnfamilies").Count();
                }
            };
            var actionList = new List<Action>(threads);
            for (var i = 0; i < threads; i++)
            {
                actionList.Add(action);
            }
            var options = new ParallelOptions();
            options.TaskScheduler = new ThreadPerTaskScheduler();
            options.MaxDegreeOfParallelism = 10000;
            var actionArray = actionList.ToArray();
            var watch = Stopwatch.StartNew();
            Parallel.Invoke(options, actionArray);
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;
        }
    }
}
