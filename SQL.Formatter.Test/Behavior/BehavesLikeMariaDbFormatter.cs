using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test.Behavior
{
    public class BehavesLikeMariaDbFormatter
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            BehavesLikeSqlFormatter.Test(formatter);
            Case.Test(formatter);
            CreateTable.Test(formatter);
            AlterTable.Test(formatter);
            Strings.Test(formatter,
                new List<string>
                {
                    StringLiteral.DoubleQuote,
                    StringLiteral.SingleQuote,
                    StringLiteral.BackQuote
                });
            Between.Test(formatter);
            Operators.Test(formatter,
                new List<string>
                {
                    "%",
                    "&",
                    "|",
                    "^",
                    "~",
                    "!=",
                    "!",
                    "<=>",
                    "<<",
                    ">>",
                    "&&",
                    "||",
                    ":="
                });
            Join.Test(formatter,
                new List<string>
                {
                    "FULL"
                },
                new List<string>
                {
                    "STRAIGHT_JOIN",
                    "NATURAL LEFT JOIN",
                    "NATURAL LEFT OUTER JOIN",
                    "NATURAL RIGHT JOIN",
                    "NATURAL RIGHT OUTER JOIN",
                });
                
            Assert.Equal(
                "SELECT\n"
                + "  a # comment\n"
                + "FROM\n"
                + "  b # comment",
                formatter.Format("SELECT a # comment\nFROM b # comment"));
            
            Assert.Equal(
                "SET\n"
                + "  @foo := (\n"
                + "    SELECT\n"
                + "      *\n"
                + "    FROM\n"
                + "      tbl\n"
                + "  );",
                formatter.Format("SET @foo := (SELECT * FROM tbl);"));
        }
    }
}