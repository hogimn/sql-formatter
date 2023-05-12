using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class SparkSqlFormatter
    {
        public readonly SqlFormatter.Formatter formatter = SqlFormatter.Of(Dialect.SparkSql);

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

        public void OperatorsTest()
        {
            Operators.Test(formatter, new List<string>
            {
                "!=",
                "%",
                "|",
                "&",
                "^",
                "~",
                "!",
                "<=>",
                "%",
                "&&",
                "||",
                "=="
            });
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(formatter, null, new List<string>
            {
                "ANTI JOIN",
                "SEMI JOIN",
                "LEFT ANTI JOIN",
                "LEFT SEMI JOIN",
                "RIGHT OUTER JOIN",
                "RIGHT SEMI JOIN",
                "NATURAL ANTI JOIN",
                "NATURAL FULL OUTER JOIN",
                "NATURAL INNER JOIN",
                "NATURAL LEFT ANTI JOIN",
                "NATURAL LEFT OUTER JOIN",
                "NATURAL LEFT SEMI JOIN",
                "NATURAL OUTER JOIN",
                "NATURAL RIGHT OUTER JOIN",
                "NATURAL RIGHT SEMI JOIN",
                "NATURAL SEMI JOIN"
            });
        }

        [Fact]
        public void FormatsWindowSpecificationAsTopLevel()
        {
            Assert.Equal(
                "SELECT\n"
                + "  *,\n"
                + "  LAG(value) OVER wnd AS next_value\n"
                + "FROM\n"
                + "  tbl\n"
                + "WINDOW\n"
                + "  wnd as (\n"
                + "    PARTITION BY\n"
                + "      id\n"
                + "    ORDER BY\n"
                + "      time\n"
                + "  );",
                formatter.Format(
                    "SELECT *, LAG(value) OVER wnd AS next_value FROM tbl WINDOW wnd as (PARTITION BY id ORDER BY time);"));
        }

        [Fact]
        public void FormatsWindowFunctionAndEndAsInline()
        {
            Assert.Equal(
                "SELECT\n"
                + "  window(time, \"1 hour\").start AS window_start,\n"
                + "  window(time, \"1 hour\").end AS window_end\n"
                + "FROM\n"
                + "  tbl;",
                formatter.Format(
                    "SELECT window(time, \"1 hour\").start AS window_start, window(time, \"1 hour\").end AS window_end FROM tbl;"));
        }

        [Fact]
        public void DoesNotAddSpacesAroundDollarParams()
        {
            Assert.Equal(
                "SELECT\n"
                + "  ${var_name};",
                formatter.Format(
                    "SELECT ${var_name};"));
        }

        [Fact]
        public void ReplacesDollarVariablesAndDollarCurlyBracketVariablesWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + "  'var one',\n"
                + "  'var two';",
                formatter.Format(
                    "SELECT $var1, ${var2};", new Dictionary<string, string>
                    {
                        { "var1", "'var one'" },
                        { "var2", "'var two'" }
                    }));
        }
    }
}