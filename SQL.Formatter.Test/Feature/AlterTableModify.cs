using System;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class AlterTableModify
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            FormatsAlterTableModifyStatement(formatter);
        }

        private static void FormatsAlterTableModifyStatement(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "ALTER TABLE\n"
                + "  supplier\n"
                + "MODIFY\n"
                + "  supplier_name char(100) NOT NULL;",
                formatter.Format("ALTER TABLE supplier MODIFY supplier_name char(100) NOT NULL;"));
        }
    }
}