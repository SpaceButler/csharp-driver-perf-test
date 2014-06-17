using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var cluster = Cluster
                .Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            var session = cluster.Connect();

            WarmUp(session);
            Console.WriteLine("Test started using {0} hosts", cluster.AllHosts().Count);
            TestThroughput(session);
            Console.WriteLine("Test finished using {0} hosts", cluster.AllHosts().Count);
            cluster.Shutdown(1000);

            Console.ReadLine();
        }

        private const int nthreads1 = 1;
        private const int nthreads2 = 5;
        private const int nthreads3 = 10;
        private const int nthreads4 = 25;
        
        private static void WarmUp(ISession session)
        {
            var test = new ThroughputTest(session);
            test.SelectSystemTest(10, 1);
        }

        private static void TestThroughput(ISession session)
        {
            var enlapsed1 = TestThroughput(session, nthreads1);
            var enlapsed2 = TestThroughput(session, nthreads2);
            var enlapsed3 = TestThroughput(session, nthreads3);
            var enlapsed4 = TestThroughput(session, nthreads4);
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Finished throughput test1");
            Console.WriteLine("  using {0} thread: {1} (Median {2})", nthreads1, enlapsed1.Average(), enlapsed1.Median());
            Console.WriteLine("  using {0} thread: {1} (Median {2})", nthreads2, enlapsed2.Average(), enlapsed2.Median());
            Console.WriteLine("  using {0} thread: {1} (Median {2})", nthreads3, enlapsed3.Average(), enlapsed3.Median());
            Console.WriteLine("  using {0} thread: {1} (Median {2})", nthreads4, enlapsed4.Average(), enlapsed4.Median());
        }

        /// <summary>
        /// Basic throughput test
        /// </summary>
        /// <param name="session"></param>
        private static List<double> TestThroughput(ISession session, int threads)
        {
            Console.WriteLine("Starting throughput test with {0} threads", threads);

            var enlapsed = new List<double>();

            var test = new ThroughputTest(session);
            //Start
            for (var i = 0; i < 10; i++ )
            {
                enlapsed.Add(test.SelectSystemTest(100, threads));
            }
            return enlapsed;
        }
    }
}
