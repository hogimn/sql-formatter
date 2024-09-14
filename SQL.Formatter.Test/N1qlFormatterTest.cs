using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class N1qlFormatterTest
    {
        public readonly SqlFormatter.Formatter Formatter = SqlFormatter.Of(Dialect.N1ql);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(Formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(Formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote
            });
        }

        [Fact]
        public void BetweenTest()
        {
            Between.Test(Formatter);
        }

        [Fact]
        public void SchemaTest()
        {
            Schema.Test(Formatter);
        }

        [Fact]
        public void OperatorsTest()
        {
            Operators.Test(Formatter, new List<string>
            {
                "%",
                "==",
                "!="
            });
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(Formatter, new List<string>
            {
                "FULL", "CROSS", "NATURAL"
            });
        }

        [Fact]
        public void ReplacesDollarNumberedPlaceholdersWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + "  second,\n"
                + "  third,\n"
                + "  first;",
                Formatter.Format(
                    "SELECT $1, $2, $0;",
                    new Dictionary<string, string>
                    {
                        { "0", "first" },
                        { "1", "second" },
                        { "2", "third" }
                    }));
        }

        [Fact]
        public void ReplacesDollarVariablesWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + @"  ""variable value""," + "\n"
                + @"  'var value'," + "\n"
                + @"  'var value'," + "\n"
                + @"  'var value';",
                Formatter.Format(
                    @"SELECT $variable, $'var name', $""var name"", $`var name`;",
                    new Dictionary<string, string>
                    {
                        { "variable", @"""variable value""" },
                        { "var name", @"'var value'" }
                    }));
        }

        [Fact]
        public void RecognizesDollarVariables()
        {
            Assert.Equal(
                "SELECT\n"
                + @"  $variable," + "\n"
                + @"  $'var name'," + "\n"
                + @"  $""var name""," + "\n"
                + @"  $`var name`;",
                Formatter.Format(
                    @"SELECT $variable, $'var name', $""var name"", $`var name`;"));
        }

        [Fact]
        public void FormatsUpdateQueryWithUseKeysAndReturning()
        {
            Assert.Equal(
                "UPDATE\n"
                + "  tutorial\n"
                + "USE KEYS\n"
                + "  'baldwin'\n"
                + "SET\n"
                + "  type = 'actor' RETURNING tutorial.type",
                Formatter.Format(
                    "UPDATE tutorial USE KEYS 'baldwin' SET type = 'actor' RETURNING tutorial.type"));
        }

        [Fact]
        public void FormatsExplainedDeleteQueryWithUseKeysAndReturning()
        {
            Assert.Equal(
                "EXPLAIN DELETE FROM\n"
                + "  tutorial t\n"
                + "USE KEYS\n"
                + "  'baldwin' RETURNING t",
                Formatter.Format(
                    "EXPLAIN DELETE FROM tutorial t USE KEYS 'baldwin' RETURNING t"));
        }

        [Fact]
        public void FormatsSelectQueryWithNestAndUseKeys()
        {
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  usr\n"
                + "USE KEYS\n"
                + "  'Elinor_33313792'\n"
                + "NEST\n"
                + "  orders_with_users orders ON KEYS ARRAY s.order_id FOR s IN usr.shipped_order_history END;",
                Formatter.Format(
                    "SELECT * FROM usr\n"
                    + "USE KEYS 'Elinor_33313792' NEST orders_with_users orders\n"
                    + "ON KEYS ARRAY s.order_id FOR s IN usr.shipped_order_history END;"));
        }

        [Fact]
        public void FormatsSelectQueryWithUnnestTopLevelReservedWord()
        {
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tutorial\n"
                + "UNNEST\n"
                + "  tutorial.children c;",
                Formatter.Format(
                    "SELECT * FROM tutorial UNNEST tutorial.children c;"));
        }

        [Fact]
        public void FormatsInsertWithLargeObjectAndArrayLiterals()
        {
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
                Formatter.Format(
                    "INSERT INTO heroes (KEY, VALUE) VALUES ('123', {'id': 1, 'type': 'Tarzan',\n"
                    + "'array': [123456789, 123456789, 123456789, 123456789, 123456789], 'hello': 'world'});"));
        }

        [Fact]
        public void FormatsInsertWithCurlyBracketObjectLiteral()
        {
            Assert.Equal(
                "INSERT INTO\n"
                + "  heroes (KEY, VALUE)\n"
                + "VALUES\n"
                + "  ('123', {'id': 1, 'type': 'Tarzan'});",
                Formatter.Format(
                    "INSERT INTO heroes (KEY, VALUE) VALUES ('123', {'id':1,'type':'Tarzan'});"));
        }

        [Fact]
        public void FormatsSelectQueryWithPrimaryKeyQuerying()
        {
            Assert.Equal("SELECT\n"
                + "  fname,\n"
                + "  email\n"
                + "FROM\n"
                + "  tutorial\n"
                + "USE KEYS\n"
                + "  ['dave', 'ian'];",
                Formatter.Format(
                    "SELECT fname, email FROM tutorial USE KEYS ['dave', 'ian'];"));
        }

        [Fact]
        public void FormatsSelectQueryWithElementSelectionExpression()
        {
            Assert.Equal(
                "SELECT\n"
                + "  order_lines[0].productId\n"
                + "FROM\n"
                + "  orders;",
                Formatter.Format(
                    "SELECT order_lines[0].productId FROM orders;"));
        }
    }
}
