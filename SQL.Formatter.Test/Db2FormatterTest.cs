using System.Collections.Generic;
using SQL.Formatter.languages;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class Db2FormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.Db2);
            BehavesLikeSqlFormatter.Test(formatter);
            CreateTable.Test(formatter);
            AlterTable.Test(formatter);
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote
            });
            Between.Test(formatter);
            Schema.Test(formatter);
            
            Assert.Equal(
                "SELECT\n"
                + "  col1\n"
                + "FROM\n"
                + "  tbl\n"
                + "ORDER BY\n"
                + "  col2 DESC\n"
                + "FETCH FIRST\n"
                + "  20 ROWS ONLY;",
                formatter.Format(
                    "SELECT col1 FROM tbl ORDER BY col2 DESC FETCH FIRST 20 ROWS ONLY;"));
        }
    }
}