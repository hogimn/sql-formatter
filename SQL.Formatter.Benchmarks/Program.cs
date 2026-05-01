using BenchmarkDotNet.Running;

namespace SQL.Formatter.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<SqlFormatterBenchmark>();
        }
    }
}
