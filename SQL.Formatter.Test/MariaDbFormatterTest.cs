using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MariaDbFormatterTest
    {
        public readonly SqlFormatter.Formatter Formatter = SqlFormatter.Of(Dialect.MariaDb);

        [Fact]
        public void BehavesLikeMariaDbFormatterTest()
        {
            BehavesLikeMariaDbFormatter.Test(Formatter);
        }
    }
}
