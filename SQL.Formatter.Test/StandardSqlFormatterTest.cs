using System.Collections.Generic;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class StandardSqlFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = SqlFormatter.Standard();
            BehavesLikeSqlFormatter.Test(formatter);
            Case.Test(formatter);
            CreateTable.Test(formatter);
            AlterTable.Test(formatter);
            Strings.Test(formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote
            });
            Between.Test(formatter);
            Schema.Test(formatter);
            Join.Test(formatter);
            
            Assert.Equal(
                "SELECT\n"
                + "  *\n"
                + "FROM\n"
                + "  table\n"
                + "  /* comment */\n"
                + "WHERE\n"
                + "  date BETWEEN '2001-01-01' AND '2010-12-31';\n"
                + "-- comment",
                formatter.Format(
                    "SELECT * FROM table/* comment */ WHERE date BETWEEN '2001-01-01' AND '2010-12-31'; -- comment"));

            Assert.Equal(
                "merge into DW_STG_USER.ACCOUNT_DIM target using (\n"
                + "  select\n"
                + "    COMMON_NAME m_commonName,\n"
                + "    ORIGIN m_origin,\n"
                + "    USAGE_TYPE m_usageType,\n"
                + "    CATEGORY m_category\n"
                + "  from\n"
                + "    MY_TABLE\n"
                + "  where\n"
                + "    USAGE_TYPE =: value\n"
                + ") source on source.m_usageType = target.USAGE_TYPE\n"
                + "when matched then\n"
                + "update\n"
                + "set\n"
                + "  target.COMMON_NAME = source.m_commonName,\n"
                + "  target.ORIGIN = source.m_origin,\n"
                + "  target.USAGE_TYPE = source.m_usageType,\n"
                + "  target.CATEGORY = source.m_category\n"
                + "where\n"
                + "  (\n"
                + "    (source.m_commonName <> target.COMMON_NAME)\n"
                + "    or (source.m_origin <> target.ORIGIN)\n"
                + "    or (source.m_usageType <> target.USAGE_TYPE)\n"
                + "    or (source.m_category <> target.CATEGORY)\n"
                + "  )\n"
                + "  when not matched then insert (\n"
                + "    target.COMMON_NAME,\n"
                + "    target.ORIGIN,\n"
                + "    target.USAGE_TYPE,\n"
                + "    target.CATEGORY\n"
                + "  )\n"
                + "values\n"
                + "  (\n"
                + "    source.m_commonName,\n"
                + "    source.m_origin,\n"
                + "    source.m_usageType,\n"
                + "    source.m_category\n"
                + "  )",
                formatter.Format(
                    "merge into DW_STG_USER.ACCOUNT_DIM target using ( select COMMON_NAME m_commonName, ORIGIN m_origin, USAGE_TYPE m_usageType, CATEGORY m_category from MY_TABLE where USAGE_TYPE = :value ) source on source.m_usageType = target.USAGE_TYPE when matched then update set target.COMMON_NAME = source.m_commonName, target.ORIGIN = source.m_origin, target.USAGE_TYPE = source.m_usageType, target.CATEGORY = source.m_category where ((source.m_commonName <> target.COMMON_NAME)or(source.m_origin <> target.ORIGIN)or(source.m_usageType <> target.USAGE_TYPE)or(source.m_category <> target.CATEGORY)) when not matched then insert ( target.COMMON_NAME, target.ORIGIN, target.USAGE_TYPE, target.CATEGORY) values (source.m_commonName, source.m_origin, source.m_usageType, source.m_category)"));
        }
    }
}