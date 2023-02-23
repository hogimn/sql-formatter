using SQL.Formatter.languages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SQL.Formatter.core.util
{
    public class RegexUtil
    {
        private static readonly string EscapeRegex =
            string.Join("|",
                new List<string> { "^", "$", "\\", ".", "*", "+", "*", "?", "(", ")", "[", "]", "{", "}", "|" }
                .Select(spChr => "(\\" + spChr + ")"));

        public static readonly Regex EscapeRegexPattern = new Regex(EscapeRegex);

        public static string EscapeRegExp(string s)
        {
            return EscapeRegexPattern.Replace(s, @"\$0");
        }

        public static string CreateOperatorRegex(JSLikeList<string> multiLetterOperators)
        {
            return string.Format("^({0}|.)",
                string.Join("|", Util.SortByLengthDesc(multiLetterOperators).Map(EscapeRegExp).ToList()));
        }

        public static string CreateLineCommentRegex(JSLikeList<string> lineCommentTypes)
        {
            return string.Format("^((?:{0}).*?)(?:\r\n|\r|\n|$)",
                string.Join("|", lineCommentTypes.Map(EscapeRegExp).ToList()));
        }

        public static string CreateReservedWordRegex(JSLikeList<string> reservedWords)
        {
            if (reservedWords.IsEmpty())
                return "^\b$";

            string reservedWordsPattern =
                string.Join("|", Util.SortByLengthDesc(reservedWords).ToList()).Replace(" ", "\\s+");

            return "(?i)" + "^(" + reservedWordsPattern + ")\\b";
        }

        public static string CreateWordRegex(JSLikeList<string> specialChars)
        {
            return "^([\\p{L}\\p{Nd}\\p{Mn}\\p{Pc}"
                + specialChars.Join("")
                + "]+)";
        }

        public static string CreateStringRegex(JSLikeList<string> stringTypes)
        {
            return "^(" + CreateStringPattern(stringTypes) + ")";
        }

        // This enables the following string patterns:
        // 1. backtick quoted string using `` to escape
        // 2. square bracket quoted string (SQL Server) using ]] to escape
        // 3. double quoted string using "" or \" to escape
        // 4. single quoted string using '' or \' to escape
        // 5. national character quoted string using N'' or N\' to escape
        public static string CreateStringPattern(JSLikeList<string> stringTypes)
        {
            return string.Join("|", stringTypes.Map(StringLiteral.Get).ToList());
        }

        public static string CreateParenRegex(JSLikeList<string> parens)
        {
            return "(?i)^(" + string.Join("|", parens.Map(EscapeParen).ToList()) + ")";
        }

        public static string EscapeParen(string paren)
        {
            if (paren.Length == 1)
            {
                // A single punctuation character
                return EscapeRegExp(paren);
            }
            // longer word
            return "\\b" + paren + "\\b";
        }

        public static Regex CreatePlaceholderRegexPattern(JSLikeList<string> types, string pattern)
        {
            if (types.IsEmpty())
                return null;

            string typesRegex = string.Join("|", types.Map(EscapeRegExp).ToList());

            return new Regex(string.Format("^((?:{0})(?:{1}))", typesRegex, pattern));
        }
    }
}
