using System;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class Case
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            FormatsCaseWhenWithABlankExpression(formatter);
        }

        private static void FormatsCaseWhenWithABlankExpression(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "CASE\n"
                + "  WHEN option = 'foo' THEN 1\n"
                + "  WHEN option = 'bar' THEN 2\n"
                + "  WHEN option = 'baz' THEN 3\n"
                + "  ELSE 4\n"
                + "END;",
                formatter.Format("CASE WHEN option = 'foo' THEN 1 WHEN option = 'bar' THEN 2 WHEN option = 'baz' THEN 3 ELSE 4 END;"));
        }
    }
}
