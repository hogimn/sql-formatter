using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQL.Formatter.languages;

namespace SQL.Formatter.Test
{
    [TestClass]
    public class SqlFormatterTest
    {
        [TestMethod]
        public void Simple()
        {
            string format = SqlFormatter.Format(
                "SELECT foo, bar, CASE baz WHEN 'one' THEN 1 WHEN 'two' THEN 2 ELSE 3 END FROM table");

            Assert.AreEqual(
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

        [TestMethod]
        public void WithIndent()
        {
            string format = SqlFormatter.Format(
                "SELECT foo, bar, CASE baz WHEN 'one' THEN 1 WHEN 'two' THEN 2 ELSE 3 END FROM table",
                "    ");
            Assert.AreEqual(
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

        [TestMethod]
        public void WithNamedParams()
        {
            Dictionary<string, string> namedParams = new Dictionary<string, string>();
            namedParams.Add("foo", "'bar'");

            string format =
                SqlFormatter.Of(Dialect.TSql).Format("SELECT * FROM tbl WHERE foo = @foo", namedParams);
            Assert.AreEqual("SELECT\n" + "  *\n" + "FROM\n" + "  tbl\n" + "WHERE\n" + "  foo = 'bar'", format);
        }

        [TestMethod]
        public void WithFatArrow()
        {
            string format =
                SqlFormatter.Extend(config => config.PlusOperators("=>"))
                .Format("SELECT * FROM tbl WHERE foo => '123'");
            Assert.AreEqual("SELECT\n" + "  *\n" + "FROM\n" + "  tbl\n" + "WHERE\n" + "  foo => '123'",
                format);
        }

        [TestMethod]
        public void WithIndexedParams()
        {
            String format = SqlFormatter.Format("SELECT * FROM tbl WHERE foo = ?", new List<string>{"'bar'"});
            Assert.AreEqual("SELECT\n" + "  *\n" + "FROM\n" + "  tbl\n" + "WHERE\n" + "  foo = 'bar'", format);
        }
    }
}
