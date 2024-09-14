using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class PlSqlFormatterTest
    {
        public readonly SqlFormatter.Formatter Formatter = SqlFormatter.Of(Dialect.PlSql);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(Formatter);
        }

        [Fact]
        public void CaseTest()
        {
            Case.Test(Formatter);
        }

        [Fact]
        public void CreateTableTest()
        {
            CreateTable.Test(Formatter);
        }

        [Fact]
        public void AlterTableTest()
        {
            AlterTable.Test(Formatter);
        }

        [Fact]
        public void AlterTableModifyTest()
        {
            AlterTableModify.Test(Formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(Formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote,
                StringLiteral.QSingleQuote
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
                "||",
                "**",
                "!=",
                ":="
            });
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(Formatter);
        }

        [Fact]
        public void FormatsOracleRecursiveSubQueriesRegardelessOfCapitalization()
        {
            Assert.Equal(
                "WITH t1(id, parent_id) AS (\n"
                + "  -- Anchor member.\n"
                + "  SELECT\n"
                + "    id,\n"
                + "    parent_id\n"
                + "  FROM\n"
                + "    tab1\n"
                + "  WHERE\n"
                + "    parent_id IS NULL\n"
                + "  MINUS\n"
                + "  -- Recursive member.\n"
                + "  SELECT\n"
                + "    t2.id,\n"
                + "    t2.parent_id\n"
                + "  FROM\n"
                + "    tab1 t2,\n"
                + "    t1\n"
                + "  WHERE\n"
                + "    t2.parent_id = t1.id\n"
                + ") SEARCH BREADTH FIRST by id SET order1,\n"
                + "another AS (\n"
                + "  SELECT\n"
                + "    *\n"
                + "  FROM\n"
                + "    dual\n"
                + ")\n"
                + "SELECT\n"
                + "  id,\n"
                + "  parent_id\n"
                + "FROM\n"
                + "  t1\n"
                + "ORDER BY\n"
                + "  order1;",
                Formatter.Format(
                    "WITH t1(id, parent_id) AS (\n"
                    + "  -- Anchor member.\n"
                    + "  SELECT\n"
                    + "    id,\n"
                    + "    parent_id\n"
                    + "  FROM\n"
                    + "    tab1\n"
                    + "  WHERE\n"
                    + "    parent_id IS NULL\n"
                    + "  MINUS\n"
                    + "    -- Recursive member.\n"
                    + "  SELECT\n"
                    + "    t2.id,\n"
                    + "    t2.parent_id\n"
                    + "  FROM\n"
                    + "    tab1 t2,\n"
                    + "    t1\n"
                    + "  WHERE\n"
                    + "    t2.parent_id = t1.id\n"
                    + ") SEARCH BREADTH FIRST by id SET order1,\n"
                    + "another AS (SELECT * FROM dual)\n"
                    + "SELECT id, parent_id FROM t1 ORDER BY order1;\n"));
        }

        [Fact]
        public void FormatsOracleRecursiveSubQueries()
        {
            Assert.Equal(
                "WITH t1(id, parent_id) AS (\n"
                + "  -- Anchor member.\n"
                + "  SELECT\n"
                + "    id,\n"
                + "    parent_id\n"
                + "  FROM\n"
                + "    tab1\n"
                + "  WHERE\n"
                + "    parent_id IS NULL\n"
                + "  MINUS\n"
                + "  -- Recursive member.\n"
                + "  SELECT\n"
                + "    t2.id,\n"
                + "    t2.parent_id\n"
                + "  FROM\n"
                + "    tab1 t2,\n"
                + "    t1\n"
                + "  WHERE\n"
                + "    t2.parent_id = t1.id\n"
                + ") SEARCH BREADTH FIRST BY id SET order1,\n"
                + "another AS (\n"
                + "  SELECT\n"
                + "    *\n"
                + "  FROM\n"
                + "    dual\n"
                + ")\n"
                + "SELECT\n"
                + "  id,\n"
                + "  parent_id\n"
                + "FROM\n"
                + "  t1\n"
                + "ORDER BY\n"
                + "  order1;",
                Formatter.Format(
                    "WITH t1(id, parent_id) AS (\n"
                    + "  -- Anchor member.\n"
                    + "  SELECT\n"
                    + "    id,\n"
                    + "    parent_id\n"
                    + "  FROM\n"
                    + "    tab1\n"
                    + "  WHERE\n"
                    + "    parent_id IS NULL\n"
                    + "  MINUS\n"
                    + "    -- Recursive member.\n"
                    + "  SELECT\n"
                    + "    t2.id,\n"
                    + "    t2.parent_id\n"
                    + "  FROM\n"
                    + "    tab1 t2,\n"
                    + "    t1\n"
                    + "  WHERE\n"
                    + "    t2.parent_id = t1.id\n"
                    + ") SEARCH BREADTH FIRST BY id SET order1,\n"
                    + "another AS (SELECT * FROM dual)\n"
                    + "SELECT id, parent_id FROM t1 ORDER BY order1;\n"));
        }

        [Fact]
        public void FormatsSelectQueryWithOuterApply()
        {
            Assert.Equal(
                "SELECT\n"
                + "  a,\n"
                + "  b\n"
                + "FROM\n"
                + "  t\n"
                + "  OUTER APPLY fn(t.id)",
                Formatter.Format(
                    "SELECT a, b FROM t OUTER APPLY fn(t.id)"));
        }

        [Fact]
        public void FormatsSimpleSelectWithNationalCharacters()
        {
            Assert.Equal(
                "SELECT\n"
                + "  N'value'",
                Formatter.Format(
                    "SELECT N'value'"));
        }

        [Fact]
        public void FormatsSimpleSelect()
        {
            Assert.Equal(
                "SELECT\n"
                + "  N,\n"
                + "  M\n"
                + "FROM\n"
                + "  t",
                Formatter.Format(
                    "SELECT N, M FROM t"));
        }

        [Fact]
        public void FormatsSelectQueryWithCrossApply()
        {
            Assert.Equal(
                "SELECT\n"
                + "  a,\n"
                + "  b\n"
                + "FROM\n"
                + "  t\n"
                + "  CROSS APPLY fn(t.id)",
                Formatter.Format(
                    "SELECT a, b FROM t CROSS APPLY fn(t.id)"));
        }

        [Fact]
        public void ReplacesQuestionmarkIndexedPlaceholdersWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + "  first,\n"
                + "  second,\n"
                + "  third;",
                Formatter.Format(
                    "SELECT ?, ?, ?;", new List<string>
                    {
                        "first",
                        "second",
                        "third"
                    }));
        }

        [Fact]
        public void ReplacesQuestionmarkNumberedPlaceholdersWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + "  second,\n"
                + "  third,\n"
                + "  first;",
                Formatter.Format(
                    "SELECT ?1, ?2, ?0;", new Dictionary<string, string>
                    {
                        { "0", "first" },
                        { "1", "second" },
                        { "2", "third" }
                    }));
        }

        [Fact]
        public void RecognizesQuestionmarkPlaceholders()
        {
            Assert.Equal(
                "SELECT\n"
                + "  ?1,\n"
                + "  ?25,\n"
                + "  ?;",
                Formatter.Format(
                    "SELECT ?1, ?25, ?;"));
        }

        [Fact]
        public void FormatsInsertWithoutInto()
        {
            Assert.Equal(
                "INSERT\n"
                + "  Customers (ID, MoneyBalance, Address, City)\n"
                + "VALUES\n"
                + "  (12, -123.4, 'Skagen 2111', 'Stv');",
                Formatter.Format(
                    "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');"));
        }

        [Fact]
        public void RecognizeSpecialCharactersAsPartOfIdentifiers()
        {
            Assert.Equal(
                "SELECT\n"
                + "  my_col$1#,\n"
                + "  col.2@\n"
                + "FROM\n"
                + "  tbl",
                Formatter.Format(
                    "SELECT my_col$1#, col.2@ FROM tbl\n"));
        }

        [Fact]
        public void FormatsOnlyAsALineComment()
        {
            Assert.Equal(
                "SELECT\n"
                + "  col\n"
                + "FROM\n"
                + "  -- This is a comment\n"
                + "  MyTable;",
                Formatter.Format(
                    "SELECT col FROM\n-- This is a comment\nMyTable;\n"));
        }

        [Fact]
        public void FormatsFetchFirstLikeLimit()
        {
            Assert.Equal(
                "SELECT\n"
                + "  col1\n"
                + "FROM\n"
                + "  tbl\n"
                + "ORDER BY\n"
                + "  col2 DESC\n"
                + "FETCH FIRST\n"
                + "  20 ROWS ONLY;",
                Formatter.Format(
                    "SELECT col1 FROM tbl ORDER BY col2 DESC FETCH FIRST 20 ROWS ONLY;"));
        }
    }
}
