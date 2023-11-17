using System.Collections.Generic;
using SQL.Formatter.Language;
using Xunit;

namespace SQL.Formatter.Test
{
    public class SqlFormatterTest
    {
        [Fact]
        public void Simple()
        {
            var format = SqlFormatter.Format(
                "SELECT foo, bar, CASE baz WHEN 'one' THEN 1 WHEN 'two' THEN 2 ELSE 3 END FROM table");

            Assert.Equal(
                "SELECT\n"
                + "  foo,\n"
                + "  bar,\n"
                + "  CASE\n"
                + "    baz\n"
                + "    WHEN 'one' THEN 1\n"
                + "    WHEN 'two' THEN 2\n"
                + "    ELSE 3\n"
                + "  END\n"
                + "FROM\n"
                + "  table",
                format);
        }

        [Fact]
        public void WithIndent()
        {
            var format = SqlFormatter.Format(
                "SELECT foo, bar, CASE baz WHEN 'one' THEN 1 WHEN 'two' THEN 2 ELSE 3 END FROM table",
                "    ");
            Assert.Equal(
                "SELECT\n"
                + "    foo,\n"
                + "    bar,\n"
                + "    CASE\n"
                + "        baz\n"
                + "        WHEN 'one' THEN 1\n"
                + "        WHEN 'two' THEN 2\n"
                + "        ELSE 3\n"
                + "    END\n"
                + "FROM\n"
                + "    table",
                format);
        }

        [Fact]
        public void WithNamedParams()
        {
            var namedParams = new Dictionary<string, string>();
            namedParams.Add("foo", "'bar'");

            var format =
                SqlFormatter.Of(Dialect.TSql).Format("SELECT * FROM tbl WHERE foo = @foo", namedParams);
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tbl\n"
                + "WHERE\n"
                + "  foo = 'bar'",
                format);
        }

        [Fact]
        public void WithFatArrow()
        {
            var format =
                SqlFormatter.Extend(config => config.PlusOperators("=>"))
                    .Format("SELECT * FROM tbl WHERE foo => '123'");
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tbl\n"
                + "WHERE\n"
                + "  foo => '123'",
                format);
        }

        [Fact]
        public void WithIndexedParams()
        {
            var format = SqlFormatter.Format("SELECT * FROM tbl WHERE foo = ?", new List<string> { "'bar'" });
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tbl\n"
                + "WHERE\n"
                + "  foo = 'bar'",
                format);
        }
    }
}
