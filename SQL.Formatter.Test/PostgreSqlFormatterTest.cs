using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class PostgreSqlFormatterTest
    {
        public readonly SqlFormatter.Formatter formatter = SqlFormatter.Of(Dialect.PostgreSql);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(formatter);
        }

        [Fact]
        public void CaseTest()
        {
            Case.Test(formatter);
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
                StringLiteral.UDoubleQuote,
                StringLiteral.USingleQuote,
                StringLiteral.Dollar
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
        public void OperatorsTest()
        {
            Operators.Test(formatter, new List<string>
            {
                "%",
                "^",
                "!",
                "!!",
                "@",
                "!=",
                "&",
                "|",
                "~",
                "#",
                "<<",
                ">>",
                "||/",
                "|/",
                "::",
                "->>",
                "->",
                "~~*",
                "~~",
                "!~~*",
                "!~~",
                "~*",
                "!~*",
                "!~",
                "@@",
                "@@@"
            });
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(formatter);
        }

        [Fact]
        public void SupportDollarPlaceholders()
        {
            Assert.Equal(
                "SELECT\n"
                + "  $1,\n"
                + "  $2\n"
                + "FROM\n"
                + "  tbl",
                formatter.Format(
                    "SELECT $1, $2 FROM tbl"));
        }

        [Fact]
        public void ReplacesDollarPlaceholdersWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + @"  ""variable value""" + ",\n"
                + @"  ""blah""" + "\n"
                + "FROM\n"
                + "  tbl",
                formatter.Format(
                    "SELECT $1, $2 FROM tbl", new Dictionary<string, string>
                    {
                        { "1", @"""variable value""" },
                        { "2", @"""blah""" }
                    }));
        }

        [Fact]
        public void SupportsColonNamePlaceholders()
        {
            Assert.Equal(
                "foo = :bar",
                formatter.Format(
                    "foo = :bar"));
        }

        [Fact]
        public void ReplacesColonNamePlaceholdersWithParamValues()
        {
            Assert.Equal(
                "foo = 'Hello'\n"
                + "AND some_col = 10\n"
                + "OR col = 7",
                formatter.Format(
                    @"foo = :bar AND :""field"" = 10 OR col = :'val'", new Dictionary<string, string>
                    {
                        { "bar", "'Hello'" },
                        { "field", "some_col" },
                        { "val", "7" }
                    }));
        }
    }
}
