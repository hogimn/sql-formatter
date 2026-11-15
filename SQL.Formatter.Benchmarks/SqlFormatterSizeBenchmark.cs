using System.Text;
using BenchmarkDotNet.Attributes;

namespace SQL.Formatter.Benchmarks
{
    [MemoryDiagnoser]
    public class SqlFormatterSizeBenchmark
    {
        private string _sql = string.Empty;

        [Params(64, 512, 4096, 16384)]
        public int SqlLength { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            const string predicate = "c.customer_id = 123 AND c.status = 'active' ";
            var sql = new StringBuilder(SqlLength);
            sql.Append("SELECT c.customer_id, c.status FROM customers c WHERE ");
            while (sql.Length < SqlLength)
            {
                sql.Append(predicate);
            }

            _sql = sql.ToString();
        }

        [Benchmark]
        public string FormatSql()
        {
            return SqlFormatter.Format(_sql);
        }
    }
}
