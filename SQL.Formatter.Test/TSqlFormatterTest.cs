using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class TSqlFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.TSql);
            BehavesLikeSqlFormatter.Test(formatter);
            Case.Test(formatter);
            CreateTable.Test(formatter);
            AlterTable.Test(formatter);
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.NSingleQuote,
                StringLiteral.Bracket
            });
            Between.Test(formatter);
            Schema.Test(formatter);
            Operators.Test(formatter, new List<string>
            {
                "%",
                "&",
                "|",
                "^",
                "~",
                "!=",
                "!<",
                "!>",
                "+=",
                "-=",
                "*=",
                "/=",
                "%=",
                "|=",
                "&=",
                "^=",
                "::"
            });

            Join.Test(formatter, new List<string>
            {
                "NATURAL"
            });

            Assert.Equal(
                "INSERT\n"
                + "  Customers (ID, MoneyBalance, Address, City)\n"
                + "VALUES\n"
                + "  (12, -123.4, 'Skagen 2111', 'Stv');",
                formatter.Format(
                    "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');"));
            
            Assert.Equal(
                "INSERT\n"
                + "  Customers (ID, MoneyBalance, Address, City)\n"
                + "VALUES\n"
                + "  (12, -123.4, 'Skagen 2111', 'Stv');",
                formatter.Format(
                    "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');"));
            
            Assert.Equal(
                "SELECT\n"
                + "  @variable,\n"
                + "  @\"var name\",\n"
                + "  @[var name];",
                formatter.Format(
                    "SELECT @variable, @\"var name\", @[var name];"));
            
            Assert.Equal(
                "SELECT\n"
                + "  'var value',\n"
                + "  'var value1',\n"
                + "  'var value2';",
                formatter.Format(
                    "SELECT @variable, @\"var name1\", @[var name2];", new Dictionary<string, string>
                    {
                        { "variable", "'var value'"},
                        { "var name1", "'var value1'"},
                        { "var name2", "'var value2'"},
                    }));

            Assert.Equal(
                "SELECT\n"
                + "  a,\n"
                + "  b\n"
                + "FROM\n"
                + "  t\n"
                + "  CROSS JOIN t2 on t.id = t2.id_t",
                formatter.Format(
                    "SELECT a, b FROM t CROSS JOIN t2 on t.id = t2.id_t"));
        }
    }
}