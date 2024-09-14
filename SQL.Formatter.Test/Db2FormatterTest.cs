using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class Db2FormatterTest
    {
        public readonly SqlFormatter.Formatter Formatter = SqlFormatter.Of(Dialect.Db2);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(Formatter);
        }

        [Fact]
        public void CreateTableTest()
        {
            CreateTable.Test(Formatter);
        }

        [Fact]
        public void AlterTableTest()
        {
            AlterTable.Test(Formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(Formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote
            });
        }

        [Fact]
        public void BetweenTest()
        {
            Between.Test(Formatter);
        }

        [Fact]
        public void SchemaTest()
        {
            Schema.Test(Formatter);
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
                Formatter.Format(
                    "SELECT col1 FROM tbl ORDER BY col2 DESC FETCH FIRST 20 ROWS ONLY;"));
        }
    }
}
