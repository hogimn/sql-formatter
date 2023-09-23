using System;
using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MySqlFormatterTest
    {
        public readonly SqlFormatter.Formatter formatter = SqlFormatter.Of(Dialect.MySql);

        [Fact]
        public void BehavesLikeMariaDbFormatterTest()
        {
            BehavesLikeMariaDbFormatter.Test(formatter);
        }

        [Fact]
        public void AdditionalMySqlOperator()
        {
            Operators.Test(formatter, new List<string>
            {
                "->",
                "->>"
            });
        }
    }
}
