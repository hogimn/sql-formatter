using BenchmarkDotNet.Attributes;

namespace SQL.Formatter.Benchmarks
{
    [MemoryDiagnoser]
    public class SqlFormatterBenchmark
    {
        private string _complexSql = "";

        [Params(1, 10, 100)]
        public int _repeatCount;

        [GlobalSetup]
        public void Setup()
        {
            _complexSql = @"
                SELECT a.id, b.name, CASE WHEN a.status = 'A' THEN 1 ELSE 0 END 
                FROM table_a a 
                INNER JOIN table_b b ON a.id = b.id 
                WHERE a.type IN ('T1', 'T2', 'T3') 
                AND a.created_at >= '2024-01-01'
                AND b.region = 'KR'
                ORDER BY a.created_at DESC, b.name ASC";
        }

        [Benchmark]
        public void RepeatFormatting()
        {
            for (var i = 0; i < _repeatCount; i++)
            {
                SqlFormatter.Format(_complexSql);
            }
        }
    }
}
