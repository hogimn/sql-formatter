using SQL.Formatter.Language;
using Xunit;

namespace SQL.Formatter.Test
{
    public class DialectTest
    {
        [Fact]
        public static void Test()
        {
            Assert.Equal(Dialect.PlSql, Dialect.NameOf("pl/sql"));
            Assert.Equal(Dialect.PlSql, Dialect.NameOf("plsql"));
        }
    }
}