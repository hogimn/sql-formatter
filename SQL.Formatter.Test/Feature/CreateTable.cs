using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class CreateTable
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            FormatsShortCreateTable(formatter);
            FormatsLongCreateTable(formatter);
        }

        private static void FormatsLongCreateTable(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "CREATE TABLE items (\n"
                + "  a INT PRIMARY KEY,\n"
                + "  b TEXT,\n"
                + "  c INT NOT NULL,\n"
                + "  doggie INT NOT NULL\n"
                + ");",
                formatter.Format("CREATE TABLE items (a INT PRIMARY KEY, b TEXT, c INT NOT NULL, doggie INT NOT NULL);"));
        }

        private static void FormatsShortCreateTable(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "CREATE TABLE items (a INT PRIMARY KEY, b TEXT);",
                formatter.Format("CREATE TABLE items (a INT PRIMARY KEY, b TEXT);"));
        }
    }
}
