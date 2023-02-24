using Xunit;

namespace SQL.Formatter.Test
{
    public class ModifiedFormatterTest
    {
        [Fact]
        public void Test()
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
            
            result = SqlFormatter.Standard()
                .Extend(config => config.PlusOperators(":="))
                .Format("SELECT * FROM TABLE WHERE A := 4");

            expect =
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  TABLE\n"
                + "WHERE\n"
                + "  A := 4";
            
            Assert.Equal(expect, result);
        }
    }
}