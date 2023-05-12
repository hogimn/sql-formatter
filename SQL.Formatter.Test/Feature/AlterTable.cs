using System;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class AlterTable
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            FormatsAlterTableAlterColumnQuery(formatter);
        }

        private static void FormatsAlterTableAlterColumnQuery(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "ALTER TABLE\n"
                + "  supplier\n"
                + "ALTER COLUMN\n"
                + "  supplier_name VARCHAR(100) NOT NULL;",
                formatter.Format("ALTER TABLE supplier ALTER COLUMN supplier_name VARCHAR(100) NOT NULL;"));
        }
    }
}