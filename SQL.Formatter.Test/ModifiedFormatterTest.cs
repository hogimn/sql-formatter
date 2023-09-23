using System;
using Xunit;

namespace SQL.Formatter.Test
{
    public class ModifiedFormatterTest
    {
        [Fact]
        public void WithColonEqualOperator()
        {
            var result = SqlFormatter.Standard()
                .Extend(config => config.PlusOperators(":="))
                .Format("SELECT * FROM TABLE WHERE A := 4");

            var expect =
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  TABLE\n"
                + "WHERE\n"
                + "  A := 4";

            Assert.Equal(expect, result);
        }

        [Fact]
        public void WithFatArrowOperator()
        {
            var result = SqlFormatter.Standard()
                .Extend(config => config.PlusOperators("=>"))
                .Format("SELECT * FROM TABLE WHERE A => 4");

            var expect =
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  TABLE\n"
                + "WHERE\n"
                + "  A => 4";

            Assert.Equal(expect, result);
        }
    }
}
