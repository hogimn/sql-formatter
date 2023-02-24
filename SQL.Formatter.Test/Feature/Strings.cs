using System.Collections.Generic;
using SQL.Formatter.languages;
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
                Assert.Equal("\"foo JOIN bar\"", formatter.Format("\"foo JOIN bar\""));
                Assert.Equal("\"foo \\\" JOIN bar\"", formatter.Format("\"foo \\\" JOIN bar\""));
            }
            
            if (stringTypes.Contains(StringLiteral.SingleQuote))
            {
                Assert.Equal("'foo JOIN bar'", formatter.Format("'foo JOIN bar'"));
                Assert.Equal("'foo \\' JOIN bar'", formatter.Format("'foo \\' JOIN bar'"));
            }
            
            if (stringTypes.Contains(StringLiteral.BackQuote))
            {
                Assert.Equal("`foo JOIN bar`", formatter.Format("`foo JOIN bar`"));
                Assert.Equal("`foo `` JOIN bar`", formatter.Format("`foo `` JOIN bar`"));
            }
            
            if (stringTypes.Contains(StringLiteral.UDoubleQuote))
            {
                Assert.Equal("U&\"foo JOIN bar\"", formatter.Format("U&\"foo JOIN bar\""));
                Assert.Equal("U&\"foo \\\" JOIN bar\"", formatter.Format("U&\"foo \\\" JOIN bar\""));
            }
            
            if (stringTypes.Contains(StringLiteral.USingleQuote))
            {
                Assert.Equal("U&'foo JOIN bar'", formatter.Format("U&'foo JOIN bar'"));
                Assert.Equal("U&'foo \\' JOIN bar'", formatter.Format("U&'foo \\' JOIN bar'"));
            }
            
            if (stringTypes.Contains(StringLiteral.Dollar))
            {
                Assert.Equal("$xxx$foo $$ LEFT JOIN $yyy$ bar$xxx$",
                    formatter.Format("$xxx$foo $$ LEFT JOIN $yyy$ bar$xxx$"));
                Assert.Equal("$$foo JOIN bar$$", formatter.Format("$$foo JOIN bar$$"));
                Assert.Equal("$$foo $ JOIN bar$$", formatter.Format("$$foo $ JOIN bar$$"));
                Assert.Equal("$$foo \n bar$$", formatter.Format("$$foo \n bar$$"));
            }

            if (stringTypes.Contains(StringLiteral.Bracket))
            {
                Assert.Equal("[foo JOIN bar]", formatter.Format("[foo JOIN bar]"));
                Assert.Equal("[foo ]] JOIN bar]", formatter.Format("[foo ]] JOIN bar]"));
            }

            if (stringTypes.Contains(StringLiteral.NSingleQuote))
            {
                Assert.Equal("N'foo JOIN bar'", formatter.Format("N'foo JOIN bar'"));
                Assert.Equal("N'foo \\' JOIN bar'", formatter.Format("N'foo \\' JOIN bar'"));
            }
            
            if (stringTypes.Contains(StringLiteral.QSingleQuote))
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
        }
    }
}