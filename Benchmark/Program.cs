using BenchmarkDotNet.Running;
using System;

namespace Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var bench = new Benchmark();
            bench.Globalsetup();
            while (true)
            {
                bench.GenerateDifficultyDiff();
            }
#else
            BenchmarkRunner.Run<Benchmark>();
#endif
        }
    }
}
