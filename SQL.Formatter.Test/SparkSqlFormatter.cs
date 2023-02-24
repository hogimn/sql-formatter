using System.Collections.Generic;
using SQL.Formatter.languages;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class SparkSqlFormatter
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.SparkSql);
            BehavesLikeSqlFormatter.Test(formatter);
            Case.Test(formatter);
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

            Assert.Equal(
                "SELECT\n"
                + "  window(time, \"1 hour\").start AS window_start,\n"
                + "  window(time, \"1 hour\").end AS window_end\n"
                + "FROM\n"
                + "  tbl;",
                formatter.Format(
                    "SELECT window(time, \"1 hour\").start AS window_start, window(time, \"1 hour\").end AS window_end FROM tbl;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  ${var_name};",
                formatter.Format(
                    "SELECT ${var_name};"));
            
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