using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class MySqlFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.MySql);
            BehavesLikeMariaDbFormatter.Test(formatter);
            Operators.Test(formatter, new List<string>
            {
                "->",
                "->>"
            });
        }
    }
}