using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class RedShiftFormatterTest
    {
        public readonly SqlFormatter.Formatter formatter = SqlFormatter.Of(Dialect.Redshift);

        [Fact]
        public void BehaviesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(formatter);
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
        public void AlterTableModifyTest()
        {
            AlterTableModify.Test(formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.BackQuote
            });
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
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(formatter);
        }

        [Fact]
        public void FormatsLimit()
        {
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
        }

        [Fact]
        public void FormatsOnlyDoubleHypenAsALineComment()
        {
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
        }

        [Fact]
        public void RecognizesAtAsPartOfIdentifiers()
        {
            Assert.Equal(
                "SELECT\n"
                + "  @col1\n"
                + "FROM\n"
                + "  tbl",
                formatter.Format(
                    "SELECT @col1 FROM tbl"));
        }

        [Fact]
        public void FormatsDiskeyAndSortkeyAfterCreateTable()
        {
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
        }

        [Fact]
        public void FormatsCopy()
        {
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
