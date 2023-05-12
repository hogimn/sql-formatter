using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class TSqlFormatterTest
    {
        public readonly SqlFormatter.Formatter formatter = SqlFormatter.Of(Dialect.TSql);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(formatter);
        }

        [Fact]
        public void CaseTest()
        {
            Case.Test(formatter);
        }

        [Fact]
        public void CreateTableTest()
        {
            CreateTable.Test(formatter);
        }

        [Fact]
        public void AlterTableTest()
        {
            AlterTable.Test(formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.NSingleQuote,
                StringLiteral.Bracket
            });
        }

        [Fact]
        public void BetweenTest()
        {
            Between.Test(formatter);
        }

        [Fact]
        public void SchemaTest()
        {
            Schema.Test(formatter);
        }

        [Fact]
        public void OperatorsTest()
        {
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
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(formatter, new List<string>
            {
                "NATURAL"
            });
        }

        [Fact]
        public void FormatsInsertWithoutInto()
        {
            Assert.Equal(
                "INSERT\n"
                + "  Customers (ID, MoneyBalance, Address, City)\n"
                + "VALUES\n"
                + "  (12, -123.4, 'Skagen 2111', 'Stv');",
                formatter.Format(
                    "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');"));
        }

        [Fact]
        public void RecognizesAtVariables()
        {
            Assert.Equal(
                "SELECT\n"
                + "  @variable,\n"
                + "  @\"var name\",\n"
                + "  @[var name];",
                formatter.Format(
                    "SELECT @variable, @\"var name\", @[var name];"));
        }

        [Fact]
        public void ReplacesAtVariablesWithParamValues()
        {
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

        [Fact]
        public void FormatsSelectQueryWithCrossJoin()
        {
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
        }
    }
}