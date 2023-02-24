using System.Collections.Generic;
using SQL.Formatter.languages;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class RedShiftFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Of(Dialect.Redshift);
            BehavesLikeSqlFormatter.Test(formatter);
            CreateTable.Test(formatter);
            AlterTable.Test(formatter);
            AlterTableModify.Test(formatter);
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote
            });
            Schema.Test(formatter);
            Operators.Test(formatter, new List<string>
            {
                "%",
                "^",
                "|/",
                "||/",
                "<<",
                ">>",
                "&",
                "|",
                "~",
                "!",
                "!=",
                "||"
            });

            Join.Test(formatter);

            Assert.Equal(
                "SELECT\n"
                + "  col1\n"
                + "FROM\n"
                + "  tbl\n"
                + "ORDER BY\n"
                + "  col2 DESC\n"
                + "LIMIT\n"
                + "  10;",
                formatter.Format(
                    "SELECT col1 FROM tbl ORDER BY col2 DESC LIMIT 10;"));

            Assert.Equal(
                "SELECT\n"
                + "  col\n"
                + "FROM\n"
                + "  -- This is a comment\n"
                + "  MyTable;",
                formatter.Format(
                    "SELECT col FROM\n"
                    + "-- This is a comment\n"
                    + "MyTable;"));

            Assert.Equal(
                "SELECT\n"
                + "  @col1\n"
                + "FROM\n"
                + "  tbl",
                formatter.Format(
                    "SELECT @col1 FROM tbl"));

            Assert.Equal(
                "CREATE TABLE items ("
                + "a INT PRIMARY KEY,"
                + " b TEXT,"
                + " c INT NOT NULL,"
                + " d INT NOT NULL"
                + ")\n"
                + "DISTKEY(created_at)\n"
                + "SORTKEY(created_at);",
                formatter.Format(
                    "CREATE TABLE items (a INT PRIMARY KEY, b TEXT, c INT NOT NULL, d INT NOT NULL) DISTKEY(created_at) SORTKEY(created_at);"));

            Assert.Equal(
                "COPY\n"
                + "  schema.table\n"
                + "FROM\n"
                + "  's3://bucket/file.csv'\n"
                + "IAM_ROLE\n"
                + "  'arn:aws:iam::123456789:role/rolename'\n"
                + "FORMAT\n"
                + "  AS CSV\n"
                + "DELIMITER\n"
                + "  ',' QUOTE '\"'\n"
                + "REGION\n"
                + "  AS 'us-east-1'",
                formatter.Format(
                    "COPY schema.table\n"
                    + "FROM 's3://bucket/file.csv'\n"
                    + "IAM_ROLE 'arn:aws:iam::123456789:role/rolename'\n"
                    + "FORMAT AS CSV DELIMITER ',' QUOTE '\"'\n"
                    + "REGION AS 'us-east-1'"));
        }
    }
}