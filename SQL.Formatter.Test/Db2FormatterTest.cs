using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class Db2FormatterTest
    {
        public readonly SqlFormatter.Formatter formatter = SqlFormatter.Of(Dialect.Db2);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(formatter);
        }

        [Fact]
        public void CreateTableTest()
        {
            CreateTable.Test(formatter);
        }

        [Fact]
        public void AlterTableTest()
        {
            AlterTable.Test(formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote
            });
        }

        [Fact]
        public void BetweenTest()
        {
            Between.Test(formatter);
        }

        [Fact]
        public void SchemaTest()
        {
            Schema.Test(formatter);
        }


        [Fact]
        public void FormatsFetchFirstLikeLimit()
        {
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
