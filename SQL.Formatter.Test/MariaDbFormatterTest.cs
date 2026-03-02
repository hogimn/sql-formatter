using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MariaDbFormatterTest
    {
        private readonly SqlFormatter.Formatter _formatter = SqlFormatter.Of(Dialect.MariaDb);
        [Fact]
        public void BehavesLikeMariaDbFormatterTest()
        {
            BehavesLikeMariaDbFormatter.Test(_formatter);
        }
    }
}
