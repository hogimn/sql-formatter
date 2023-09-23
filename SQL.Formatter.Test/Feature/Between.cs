using System;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class Between
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            FormatsBetweenAndOnSingleLine(formatter);
        }

        private static void FormatsBetweenAndOnSingleLine(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("foo BETWEEN bar AND baz",
                formatter.Format("foo BETWEEN bar AND baz"));
        }
    }
}
