using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MySqlFormatterTest
    {
        private readonly SqlFormatter.Formatter _formatter = SqlFormatter.Of(Dialect.MySql);

        [Fact]
        public void BehavesLikeMariaDbFormatterTest()
        {
            BehavesLikeMariaDbFormatter.Test(_formatter);
        }

        [Fact]
        public void AdditionalMySqlOperator()
        {
            Operators.Test(_formatter, new List<string>
            {
                "->",
                "->>"
            });
        }
    }
}
