using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MariaDbFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.MariaDb);
            BehavesLikeMariaDbFormatter.Test(formatter);
        }
    }
}