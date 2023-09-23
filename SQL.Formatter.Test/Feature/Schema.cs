using System;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class Schema
    {
        public static void Test(SqlFormatter.Formatter formatter)
        {
            FormatsSimpleSetSchemaStatements(formatter);
        }

        private static void FormatsSimpleSetSchemaStatements(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "SET SCHEMA\n"
                + "  schema1;",
                formatter.Format("SET SCHEMA schema1;"));
        }
    }
}
