using System.Collections.Generic;
using SQL.Formatter.languages;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class PostgreSqlFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.PostgreSql);
            BehavesLikeSqlFormatter.Test(formatter);
            Case.Test(formatter);
            CreateTable.Test(formatter);
            AlterTable.Test(formatter);
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.UDoubleQuote,
                StringLiteral.USingleQuote,
                StringLiteral.Dollar
            });
            Between.Test(formatter);
            Schema.Test(formatter);
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

            Join.Test(formatter);

            Assert.Equal(
                "SELECT\n"
                + "  $1,\n"
                + "  $2\n"
                + "FROM\n"
                + "  tbl",
                formatter.Format(
                    "SELECT $1, $2 FROM tbl"));

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

            Assert.Equal(
                "foo = :bar",
                formatter.Format(
                    "foo = :bar"));

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