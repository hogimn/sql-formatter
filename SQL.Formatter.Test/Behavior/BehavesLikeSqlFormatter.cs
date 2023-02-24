using Xunit;

namespace SQL.Formatter.Test.Behavior
{
    public class BehavesLikeSqlFormatter
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(";", formatter.Format(";"));
            
            Assert.Equal(
                "SELECT\n"
                + "  count(*),\n"
                + "  Column1\n"
                + "FROM\n"
                + "  Table1;",
                formatter.Format("SELECT count(*),Column1 FROM Table1;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  DISTINCT name,\n"
                + "  ROUND(age / 7) field1,\n"
                + "  18 + 20 AS field2,\n"
                + "  'some string'\n"
                + "FROM\n"
                + "  foo;",
                formatter.Format("SELECT DISTINCT name, ROUND(age/7) field1, 18 + 20 AS field2, 'some string' FROM foo;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  foo\n"
                + "WHERE\n"
                + "  Column1 = 'testing'\n"
                + "  AND (\n"
                + "    (\n"
                + "      Column2 = Column3\n"
                + "      OR Column4 >= NOW()\n"
                + "    )\n"
                + "  );",
                formatter.Format(
                    "SELECT * FROM foo WHERE Column1 = 'testing'\n"
                    + "AND ( (Column2 = Column3 OR Column4 >= NOW()) );"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  foo\n"
                + "WHERE\n"
                + "  name = 'John'\n"
                + "GROUP BY\n"
                + "  some_column\n"
                + "HAVING\n"
                + "  column > 10\n"
                + "ORDER BY\n"
                + "  other_column\n"
                + "LIMIT\n"
                + "  5;",
                formatter.Format(
                    "SELECT * FROM foo WHERE name = 'John' GROUP BY some_column\n"
                    + "HAVING column > 10 ORDER BY other_column LIMIT 5;"));

            Assert.Equal(
                "LIMIT\n"
                + "  5;\n"
                + "SELECT\n"
                + "  foo,\n"
                + "  bar;",
                formatter.Format(
                    "LIMIT 5; SELECT foo, bar;"));
            
            Assert.Equal(
                "limit\n"
                + "  5, 10;",
                formatter.Format(
                    "limit 5, 10;"));
            
            Assert.Equal(
                "select\n"
                + "  distinct *\n"
                + "frOM\n"
                + "  foo\n"
                + "WHERe\n"
                + "  a > 1\n"
                + "  and b = 3",
                formatter.Format(
                    "select distinct * frOM foo WHERe a > 1 and b = 3"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *,\n"
                + "  SUM(*) AS sum\n"
                + "FROM\n"
                + "  (\n"
                + "    SELECT\n"
                + "      *\n"
                + "    FROM\n"
                + "      Posts\n"
                + "    LIMIT\n"
                + "      30\n"
                + "  )\n"
                + "WHERE\n"
                + "  a > b",
                formatter.Format(
                    "SELECT *, SUM(*) AS sum FROM (SELECT * FROM Posts LIMIT 30) WHERE a > b"));
            
            Assert.Equal(
                "INSERT INTO\n"
                + "  Customers (ID, MoneyBalance, Address, City)\n"
                + "VALUES\n"
                + "  (12, -123.4, 'Skagen 2111', 'Stv');",
                formatter.Format(
                    "INSERT INTO Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');"));
            
            Assert.Equal(
                "WITH TestIds AS (\n"
                + "  VALUES\n"
                + "    (4),\n"
                + "    (5),\n"
                + "    (6),\n"
                + "    (7),\n"
                + "    (9),\n"
                + "    (10),\n"
                + "    (11)\n"
                + ")\n"
                + "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  TestIds;",
                formatter.Format(
                    "WITH TestIds AS (VALUES (4),(5), (6),(7),(9),(10),(11)) SELECT * FROM TestIds;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  (a + b * (c - NOW()));",
                formatter.Format(
                    "SELECT (a + b * (c - NOW()));"));
            
            Assert.Equal(
                "INSERT INTO\n"
                + "  some_table (\n"
                + "    id_product,\n"
                + "    id_shop,\n"
                + "    id_currency,\n"
                + "    id_country,\n"
                + "    id_registration\n"
                + "  ) (\n"
                + "    SELECT\n"
                + "      IF(\n"
                + "        dq.id_discounter_shopping = 2,\n"
                + "        dq.value,\n"
                + "        dq.value / 100\n"
                + "      ),\n"
                + "      IF (\n"
                + "        dq.id_discounter_shopping = 2,\n"
                + "        'amount',\n"
                + "        'percentage'\n"
                + "      )\n"
                + "    FROM\n"
                + "      foo\n"
                + "  );",
                formatter.Format(
                    "INSERT INTO some_table (id_product, id_shop, id_currency, id_country, id_registration) (\n"
                    + "SELECT IF(dq.id_discounter_shopping = 2, dq.value, dq.value / 100),\n"
                    + "IF (dq.id_discounter_shopping = 2, 'amount', 'percentage') FROM foo);"));
            
            Assert.Equal(
                "UPDATE\n"
                + "  Customers\n"
                + "SET\n"
                + "  ContactName = 'Alfred Schmidt',\n"
                + "  City = 'Hamburg'\n"
                + "WHERE\n"
                + "  CustomerName = 'Alfreds Futterkiste';",
                formatter.Format(
                    "UPDATE Customers SET ContactName='Alfred Schmidt', City='Hamburg' WHERE CustomerName='Alfreds Futterkiste';"));
            
            Assert.Equal(
                "DELETE FROM\n"
                + "  Customers\n"
                + "WHERE\n"
                + "  CustomerName = 'Alfred'\n"
                + "  AND Phone = 5002132;",
                formatter.Format(
                    "DELETE FROM Customers WHERE CustomerName='Alfred' AND Phone=5002132;"));
            
            Assert.Equal(
                "DROP TABLE IF EXISTS admin_role;",
                formatter.Format(
                    "DROP TABLE IF EXISTS admin_role;"));
            
            Assert.Equal(
                "UPDATE\n"
                + "  customers\n"
                + "SET\n"
                + "  total_orders = order_summary.total\n"
                + "FROM\n"
                + "  (\n"
                + "    SELECT\n"
                + "      *\n"
                + "    FROM\n"
                + "      bank\n"
                + "  ) AS order_summary",
                formatter.Format(
                    "UPDATE customers SET total_orders = order_summary.total  FROM ( SELECT * FROM bank) AS order_summary"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  foo\n"
                + "  LEFT JOIN bar\n"
                + "ORDER BY\n"
                + "  blah",
                formatter.Format(
                    "SELECT * FROM foo LEFT \t   \n JOIN bar ORDER \n BY blah"));
            
            Assert.Equal(
                "(\n"
                + "  (\n"
                + "    foo = '0123456789-0123456789-0123456789-0123456789'\n"
                + "  )\n"
                + ")",
                formatter.Format(
                    "((foo = '0123456789-0123456789-0123456789-0123456789'))"));
            
            Assert.Equal("foo ALL bar", formatter.Format("foo ALL bar"));
            Assert.Equal("foo = ANY (1, 2, 3)", formatter.Format("foo = ANY (1, 2, 3)"));
            Assert.Equal("EXISTS bar", formatter.Format("EXISTS bar"));
            Assert.Equal("foo IN (1, 2, 3)", formatter.Format("foo IN (1, 2, 3)"));
            Assert.Equal("foo LIKE 'hello%'", formatter.Format("foo LIKE 'hello%'"));
            Assert.Equal("foo IS NULL", formatter.Format("foo IS NULL"));
            Assert.Equal("UNIQUE foo", formatter.Format("UNIQUE foo"));
            
            Assert.Equal("foo\nAND bar", formatter.Format("foo AND bar"));
            Assert.Equal("foo\nOR bar", formatter.Format("foo OR bar"));
            
            Assert.Equal("foo;\nbar;", formatter.Format("foo;bar;"));
            Assert.Equal("foo;\nbar;", formatter.Format("foo\n;bar;"));
            Assert.Equal("foo;\nbar;", formatter.Format("foo\n\n\n;bar;\n\n"));
            
            Assert.Equal(
                "SELECT\n"
                + "  count(*),\n"
                + "  Column1\n"
                + "FROM\n"
                + "  Table1;\n"
                + "SELECT\n"
                + "  count(*),\n"
                + "  Column1\n"
                + "FROM\n"
                + "  Table2;",
                formatter.Format(
                    "SELECT count(*),Column1 FROM Table1;\n"
                    + "SELECT count(*),Column1 FROM Table2;"));

            Assert.Equal(
                "SELECT\n"
                + "  结合使用,\n"
                + "  тест,\n"
                + "  안녕하세요,\n"
                + "  最高です\n"
                + "FROM\n"
                + "  table;",
                formatter.Format(
                    "SELECT 结合使用, тест, 안녕하세요, 最高です FROM table;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  test;\n"
                + "CREATE TABLE TEST(\n"
                + "  id NUMBER NOT NULL,\n"
                + "  col1 VARCHAR2(20),\n"
                + "  col2 VARCHAR2(20)\n"
                + ");", 
                formatter.Format(
                    "SELECT * FROM test;\n"
                    + "CREATE TABLE TEST(id NUMBER NOT NULL, col1 VARCHAR2(20), col2 VARCHAR2(20));"));

            Assert.Equal(
                "SELECT\n"
                + "  1e-9 AS a,\n"
                + "  1.5e-10 AS b,\n"
                + "  3.5E12 AS c,\n"
                + "  3.5e12 AS d;",
                formatter.Format(
                    "SELECT 1e-9 AS a, 1.5e-10 AS b, 3.5E12 AS c, 3.5e12 AS d;"));
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tbl1\n"
                + "UNION ALL\n"
                + "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  tbl2;", 
                formatter.Format(
                    "SELECT * FROM tbl1\n"
                    + "UNION ALL\n"
                    + "SELECT * FROM tbl2;"));
        }
    }
}