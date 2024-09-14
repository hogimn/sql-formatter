using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MySqlFormatterTest
    {
        public readonly SqlFormatter.Formatter Formatter = SqlFormatter.Of(Dialect.MySql);

        [Fact]
        public void BehavesLikeMariaDbFormatterTest()
        {
            BehavesLikeMariaDbFormatter.Test(Formatter);
        }

        [Fact]
        public void AdditionalMySqlOperator()
        {
            Operators.Test(Formatter, new List<string>
            {
                "->",
                "->>"
            });
        }
    }
}
