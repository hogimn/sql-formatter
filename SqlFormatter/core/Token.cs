using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core
{
    public class Token
    {
        public readonly TokenTypes type;
        public readonly string value;
        public readonly string regex;
        public readonly string whitespaceBefore;
        public readonly string key;

        public Token(TokenTypes type, string value, string regex, string whitespaceBefore, string key)
        {
            this.type = type;
            this.value = value;
            this.regex = regex;
            this.whitespaceBefore = whitespaceBefore;
            this.key = key;
        }

        public Token(TokenTypes type, string value, string regex, string whitespaceBefore)
            : this(type, value, regex, whitespaceBefore, null) { }

        public Token(TokenTypes type, string value, string regex)
            : this(type, value ,regex, null) { }

        public Token(TokenTypes type, string value)
            : this(type, value, null) { }

        public Token WithWhitespaceBefore(string whitespaceBefore)
        {
            return new Token(type, value, regex, whitespaceBefore, key);
        }

        public Token WithKey(string key)
        {
            return new Token(type, value, regex, whitespaceBefore, key);
        }

        public override string ToString()
        {
            return "type: " + type + ", value: [" + value + "], regex: /" + regex + "/, key:" + key;
        }

        private static readonly Regex AND =
            new Regex("^AND$", RegexOptions.IgnoreCase);
        private static readonly Regex BETWEEN =
            new Regex("^BETWEEN$", RegexOptions.IgnoreCase);
        private static readonly Regex LIMIT =
            new Regex("^LIMIT$", RegexOptions.IgnoreCase);
        private static readonly Regex SET =
            new Regex("^SET$", RegexOptions.IgnoreCase);
        private static readonly Regex BY =
            new Regex("^BY$", RegexOptions.IgnoreCase);
        private static readonly Regex WINDOW =
            new Regex("^WINDOW$", RegexOptions.IgnoreCase);
        private static readonly Regex END =
            new Regex("^END$", RegexOptions.IgnoreCase);

        private static Func<Token, bool> IsToken(TokenTypes type, Regex regex)
        { 
            return token => token.type == type && regex.IsMatch(token.value);
        }

        public static bool IsAnd(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.RESERVED_NEWLINE, AND).Invoke(token);
        }

        public static bool IsBetween(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.RESERVED, BETWEEN).Invoke(token);
        }

        public static bool IsLimit(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, LIMIT).Invoke(token);
        }

        public static bool IsSet(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, SET).Invoke(token);
        }

        public static bool IsBy(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.RESERVED, BY).Invoke(token);
        }

        public static bool IsWindow(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, WINDOW).Invoke(token);
        }

        public static bool IsEnd(Token token)
        {
            if (token == null) return false;
            return IsToken(TokenTypes.CLOSE_PAREN, END).Invoke(token);
        }
    }
}
