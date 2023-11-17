using SQL.Formatter.Language;
using Xunit;

namespace SQL.Formatter.Test
{
    public class DialectTest
    {
        [Fact]
        public void FindDialectByNameOrAlias()
        {
            Assert.Equal(Dialect.PlSql, Dialect.NameOf("pl/sql"));
            Assert.Equal(Dialect.PlSql, Dialect.NameOf("plsql"));
        }
    }
}
