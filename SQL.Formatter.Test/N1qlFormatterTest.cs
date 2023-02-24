using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class N1qlFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.N1ql);
            BehavesLikeSqlFormatter.Test(formatter);
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
                "%",
                "==",
                "!="
            });
            Join.Test(formatter, new List<string>
            {
                "FULL", "CROSS", "NATURAL"
            });
            
            Assert.Equal(
                "SELECT\n"
                + "  order_lines[0].productId\n"
                + "FROM\n"
                + "  orders;",
                formatter.Format(
                    "SELECT order_lines[0].productId FROM orders;"));
                    
            Assert.Equal(
                "SELECT\n"
                + "  fname,\n"
                + "  email\n"
                + "FROM\n"
                + "  tutorial\n"
                + "USE KEYS\n"
                + "  ['dave', 'ian'];",
                formatter.Format(
                    "SELECT fname, email FROM tutorial USE KEYS ['dave', 'ian'];"));
            
            Assert.Equal(
                "INSERT INTO\n"
                + "  heroes (KEY, VALUE)\n"
                + "VALUES\n"
                + "  ('123', {'id': 1, 'type': 'Tarzan'});",
                formatter.Format(
                    "INSERT INTO heroes (KEY, VALUE) VALUES ('123', {'id':1,'type':'Tarzan'});"));

            Assert.Equal(
                "INSERT INTO\n"
                + "  heroes (KEY, VALUE)\n"
                + "VALUES\n"
                + "  (\n"
                + "    '123',\n"
                + "    {\n"
                + "      'id': 1,\n"
                + "      'type': 'Tarzan',\n"
                + "      'array': [\n"
                + "        123456789,\n"
                + "        123456789,\n"
                + "        123456789,\n"
                + "        123456789,\n"
                + "        123456789\n"
                + "      ],\n"
                + "      'hello': 'world'\n"
                + "    }\n"
                + "  );",
                formatter.Format(
                    "INSERT INTO heroes (KEY, VALUE) VALUES ('123', {'id': 1, 'type': 'Tarzan',\n"
                    + "'array': [123456789, 123456789, 123456789, 123456789, 123456789], 'hello': 'world'});"));

            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tutorial\n"
                + "UNNEST\n"
                + "  tutorial.children c;",
                formatter.Format(
                    "SELECT * FROM tutorial UNNEST tutorial.children c;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  usr\n"
                + "USE KEYS\n"
                + "  'Elinor_33313792'\n"
                + "NEST\n"
                + "  orders_with_users orders ON KEYS ARRAY s.order_id FOR s IN usr.shipped_order_history END;",
                formatter.Format(
                    "SELECT * FROM usr\n"
                    + "USE KEYS 'Elinor_33313792' NEST orders_with_users orders\n"
                    + "ON KEYS ARRAY s.order_id FOR s IN usr.shipped_order_history END;"));

            Assert.Equal(
                "EXPLAIN DELETE FROM\n"
                + "  tutorial t\n"
                + "USE KEYS\n"
                + "  'baldwin' RETURNING t",
                formatter.Format(
                    "EXPLAIN DELETE FROM tutorial t USE KEYS 'baldwin' RETURNING t"));
            
            Assert.Equal(
                "UPDATE\n"
                + "  tutorial\n"
                + "USE KEYS\n"
                + "  'baldwin'\n"
                + "SET\n"
                + "  type = 'actor' RETURNING tutorial.type",
                formatter.Format(
                    "UPDATE tutorial USE KEYS 'baldwin' SET type = 'actor' RETURNING tutorial.type"));
            
            Assert.Equal(
                "SELECT\n"
                + @"  $variable," + "\n"
                + @"  $'var name'," + "\n"
                + @"  $""var name""," + "\n"
                + @"  $`var name`;",
                formatter.Format(
                    @"SELECT $variable, $'var name', $""var name"", $`var name`;"));
            
            Assert.Equal(
                "SELECT\n"
                + @"  ""variable value""," + "\n"
                + @"  'var value'," + "\n"
                + @"  'var value'," + "\n"
                + @"  'var value';",
                formatter.Format(
                    @"SELECT $variable, $'var name', $""var name"", $`var name`;",
                    new Dictionary<string, string>
                    {
                        { "variable", @"""variable value""" },
                        { "var name", @"'var value'" }
                    }));
            
            Assert.Equal(
                "SELECT\n"
                + "  second,\n"
                + "  third,\n"
                + "  first;",
                formatter.Format(
                    "SELECT $1, $2, $0;",
                    new Dictionary<string, string>
                    {
                        { "0", "first" },
                        { "1", "second" },
                        { "2", "third" }
                    }));
        }
    }
}