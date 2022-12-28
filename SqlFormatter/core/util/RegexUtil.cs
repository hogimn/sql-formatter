using VerticalBlank.SqlFormatter.languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core.util
{
    public class RegexUtil
    {
        private static readonly string ESCAPE_REGEX =
            (new List<string>{ "^", "$", "\\", ".", "*", "+", "*", "?", "(", ")", "[", "]", "{", "}", "|" })
            .Select(spChr => "(\\" + spChr + ")")
            .Aggregate((x, y) => x + "|" + y);

        public static readonly Regex ESCAPE_REGEX_PATTERN = new Regex(ESCAPE_REGEX);

        public static string EscapeRegExp(string s)
        {
            return ESCAPE_REGEX_PATTERN.Replace(s, @"\$0");
        }

        public static string CreateOperatorRegex(JSLikeList<string> multiLetterOperators)
        {
            return string.Format("^({0}|.)",
                Util.SortByLengthDesc(multiLetterOperators).Map(EscapeRegExp).ToList()
                .Aggregate((x, y) => x + "|" + y));
        }

        public static string CreateLineCommentRegex(JSLikeList<string> lineCommentTypes)
        {
            return string.Format("^((?:{0}).*?)(?:\r\n|\r|\n|$)",
                lineCommentTypes.Map(EscapeRegExp).ToList()
                .Aggregate((x, y) => x + "|" + y));
        }

        public static string CreateReservedWordRegex(JSLikeList<string> reservedWords)
        { 
            if (reservedWords.IsEmpty())
                return "^\b$";

            string reservedWordsPattern =
                Util.SortByLengthDesc(reservedWords).ToList()
                .Aggregate((x, y) => x + "|" + y).Replace(" ", "\\s+");

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
            return stringTypes.Map(StringLiteral.Get).ToList()
                .Aggregate((x, y) => (x + "|" + y));
        }

        public static string CreateParenRegex(JSLikeList<string> parens)
        { 
            return "(?i)^(" + parens.Map(EscapeParen).ToList()
                .Aggregate((x, y) => x + "|" + y) + ")";
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

            string typesRegex = types.Map(EscapeRegExp).ToList()
                .Aggregate((x, y) => x + "|" + y);

            return new Regex(string.Format("^((?:{0})(?:{1}))", typesRegex, pattern));
        }
    }
}
