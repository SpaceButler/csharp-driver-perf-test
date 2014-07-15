using System.Reflection;
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
            Console.WriteLine("Driver version {0}", Assembly.GetAssembly(typeof (ISession)).GetName().Version);
            Console.WriteLine("Test started using {0} hosts, protocol version {1} used", cluster.AllHosts().Count, session.BinaryProtocolVersion);
            TestThroughput(session);
            TestThroughputAsync(session);
            Console.WriteLine("Test finished using {0} hosts", cluster.AllHosts().Count);
            cluster.Shutdown(1000);

            Console.ReadLine();
        }

        private const int Nthreads1 = 1;
        private const int Nthreads2 = 5;
        private const int Nthreads3 = 10;
        private const int Nthreads4 = 25;

        private const int NOpsInParallel1 = 10;
        private const int NOpsInParallel2 = 50;
        private const int NOpsInParallel3 = 100;
        private const int NOpsInParallel4 = 250;
        
        private static void WarmUp(ISession session)
        {
            var test = new ThroughputTest(session);
            test.SelectSystemTest(10, 1);
        }

        private static void TestThroughput(ISession session)
        {
            Console.WriteLine("Starting throughput sync test");
            var enlapsed1 = TestThroughput(session, Nthreads1);
            var enlapsed2 = TestThroughput(session, Nthreads2);
            var enlapsed3 = TestThroughput(session, Nthreads3);
            var enlapsed4 = TestThroughput(session, Nthreads4);
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Finished throughput sync test");
            Console.WriteLine("  n iterations using {0} threads: {1} (Median {2})", Nthreads1, enlapsed1.Average(), enlapsed1.Median());
            Console.WriteLine("  n iterations using {0} threads: {1} (Median {2})", Nthreads2, enlapsed2.Average(), enlapsed2.Median());
            Console.WriteLine("  n iterations using {0} threads: {1} (Median {2})", Nthreads3, enlapsed3.Average(), enlapsed3.Median());
            Console.WriteLine("  n iterations using {0} threads: {1} (Median {2})", Nthreads4, enlapsed4.Average(), enlapsed4.Median());
        }

        private static void TestThroughputAsync(ISession session)
        {
            Console.WriteLine("Starting throughput async test");
            var enlapsed1 = TestThroughputAsync(session, NOpsInParallel1);
            var enlapsed2 = TestThroughputAsync(session, NOpsInParallel2);
            var enlapsed3 = TestThroughputAsync(session, NOpsInParallel3);
            var enlapsed4 = TestThroughputAsync(session, NOpsInParallel4);
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Finished throughput async test");
            Console.WriteLine("  n iterations using {0} ops in parallel: {1} (Median {2})", NOpsInParallel1, enlapsed1.Average(), enlapsed1.Median());
            Console.WriteLine("  n iterations using {0} ops in parallel: {1} (Median {2})", NOpsInParallel2, enlapsed2.Average(), enlapsed2.Median());
            Console.WriteLine("  n iterations using {0} ops in parallel: {1} (Median {2})", NOpsInParallel3, enlapsed3.Average(), enlapsed3.Median());
            Console.WriteLine("  n iterations using {0} ops in parallel: {1} (Median {2})", NOpsInParallel4, enlapsed4.Average(), enlapsed4.Median());
        }

        /// <summary>
        /// Basic throughput test
        /// </summary>
        private static List<double> TestThroughput(ISession session, int threads)
        {
            Console.WriteLine("Starting throughput test with {0} threads", threads);

            var enlapsed = new List<double>();

            var test = new ThroughputTest(session);
            //Start
            for (var i = 0; i < 10; i++)
            {
                enlapsed.Add(test.SelectSystemTest(100, threads));
            }
            return enlapsed;
        }

        /// <summary>
        /// Basic throughput of async ops
        /// </summary>
        private static List<double> TestThroughputAsync(ISession session, int opsInParallel)
        {
            var enlapsed = new List<double>();

            var test = new ThroughputTest(session);
            //Start
            for (var i = 0; i < 10; i++)
            {
                enlapsed.Add(test.SelectSystemTestAsync(10, opsInParallel));
            }
            return enlapsed;
        }
    }
}
