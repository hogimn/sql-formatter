using System;
using System.Collections.Generic;
using SQL.Formatter.Language;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class Strings
    {
        public static void Test(SqlFormatter.Formatter formatter, List<string> stringTypes)
        {
            if (stringTypes == null) stringTypes = new List<string>();

            if (stringTypes.Contains(StringLiteral.DoubleQuote))
            {
                SupportsDoubleQuotedStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.SingleQuote))
            {
                SupportsSingleQuotedStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.BackQuote))
            {
                SupportsBackTickQuotedStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.UDoubleQuote))
            {
                SupportsUnicodeDoubleQuotedStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.USingleQuote))
            {
                SupportsUnicodeSingleQuotedStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.Dollar))
            {
                SupportsDollarQuotedStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.Bracket))
            {
                SupportsBacketQuotedIdentifiers(formatter);
            }

            if (stringTypes.Contains(StringLiteral.NSingleQuote))
            {
                SupportsTSqlUnicodeStrings(formatter);
            }

            if (stringTypes.Contains(StringLiteral.QSingleQuote))
            {
                SupportsOracleQuotationOperator(formatter);
            }
        }

        private static void SupportsOracleQuotationOperator(SqlFormatter.Formatter formatter)
        {
            Assert.Equal(
                "Q'[I'm boy]',\n"
                + "Q'{I'm boy}',\n"
                + "Q'<I'm boy>',\n"
                + "Q'(I'm boy)',\n"
                + "1",
                formatter.Format("Q'[I'm boy]',Q'{I'm boy}',Q'<I'm boy>',Q'(I'm boy)',1"));

            Assert.Equal(
                "NQ'[I'm boy]',\n"
                + "NQ'{I'm boy}',\n"
                + "NQ'<I'm boy>',\n"
                + "NQ'(I'm boy)',\n"
                + "1",
                formatter.Format("NQ'[I'm boy]',NQ'{I'm boy}',NQ'<I'm boy>',NQ'(I'm boy)',1"));
        }

        private static void SupportsTSqlUnicodeStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("N'foo JOIN bar'", formatter.Format("N'foo JOIN bar'"));
            Assert.Equal("N'foo \\' JOIN bar'", formatter.Format("N'foo \\' JOIN bar'"));
        }

        private static void SupportsBacketQuotedIdentifiers(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("[foo JOIN bar]", formatter.Format("[foo JOIN bar]"));
            Assert.Equal("[foo ]] JOIN bar]", formatter.Format("[foo ]] JOIN bar]"));
        }

        private static void SupportsDollarQuotedStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("$xxx$foo $$ LEFT JOIN $yyy$ bar$xxx$",
                formatter.Format("$xxx$foo $$ LEFT JOIN $yyy$ bar$xxx$"));
            Assert.Equal("$$foo JOIN bar$$", formatter.Format("$$foo JOIN bar$$"));
            Assert.Equal("$$foo $ JOIN bar$$", formatter.Format("$$foo $ JOIN bar$$"));
            Assert.Equal("$$foo \n bar$$", formatter.Format("$$foo \n bar$$"));
        }

        private static void SupportsUnicodeSingleQuotedStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("U&'foo JOIN bar'", formatter.Format("U&'foo JOIN bar'"));
            Assert.Equal("U&'foo \\' JOIN bar'", formatter.Format("U&'foo \\' JOIN bar'"));
        }

        private static void SupportsUnicodeDoubleQuotedStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("U&\"foo JOIN bar\"", formatter.Format("U&\"foo JOIN bar\""));
            Assert.Equal("U&\"foo \\\" JOIN bar\"", formatter.Format("U&\"foo \\\" JOIN bar\""));
        }

        private static void SupportsBackTickQuotedStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("`foo JOIN bar`", formatter.Format("`foo JOIN bar`"));
            Assert.Equal("`foo `` JOIN bar`", formatter.Format("`foo `` JOIN bar`"));
        }

        private static void SupportsSingleQuotedStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("'foo JOIN bar'", formatter.Format("'foo JOIN bar'"));
            Assert.Equal("'foo \\' JOIN bar'", formatter.Format("'foo \\' JOIN bar'"));
        }

        private static void SupportsDoubleQuotedStrings(SqlFormatter.Formatter formatter)
        {
            Assert.Equal("\"foo JOIN bar\"", formatter.Format("\"foo JOIN bar\""));
            Assert.Equal("\"foo \\\" JOIN bar\"", formatter.Format("\"foo \\\" JOIN bar\""));
        }
    }
}