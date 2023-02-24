using System.Collections.Generic;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class Operators
    {
        public static void Test(SqlFormatter.Formatter formatter, List<string> operators)
        {
            operators.ForEach(op =>
            {
                Assert.Equal(
                    $"foo {op} bar",
                    formatter.Format($"foo{op}bar"));
            });
        }
    }
}