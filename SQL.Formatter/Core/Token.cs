using System;
using System.Text.RegularExpressions;

namespace SQL.Formatter.Core
{
    public class Token
    {
        public readonly TokenTypes Type;
        public readonly string Value;
        public readonly string Regex;
        public readonly string WhitespaceBefore;
        public readonly string Key;

        public Token(TokenTypes type, string value, string regex, string whitespaceBefore, string key)
        {
            Type = type;
            Value = value;
            Regex = regex;
            WhitespaceBefore = whitespaceBefore;
            Key = key;
        }

        public Token(TokenTypes type, string value, string regex, string whitespaceBefore)
            : this(type, value, regex, whitespaceBefore, null) { }

        public Token(TokenTypes type, string value, string regex)
            : this(type, value, regex, null) { }

        public Token(TokenTypes type, string value)
            : this(type, value, null) { }

        public Token WithWhitespaceBefore(string whitespaceBefore)
        {
            return new Token(Type, Value, Regex, whitespaceBefore, Key);
        }

        public Token WithKey(string key)
        {
            return new Token(Type, Value, Regex, WhitespaceBefore, key);
        }

        public override string ToString()
        {
            return "type: " + Type + ", value: [" + Value + "], regex: /" + Regex + "/, key:" + Key;
        }

        private static readonly Regex s_and =
            new Regex("^AND$", RegexOptions.IgnoreCase);
        private static readonly Regex s_between =
            new Regex("^BETWEEN$", RegexOptions.IgnoreCase);
        private static readonly Regex s_limit =
            new Regex("^LIMIT$", RegexOptions.IgnoreCase);
        private static readonly Regex s_set =
            new Regex("^SET$", RegexOptions.IgnoreCase);
        private static readonly Regex s_by =
            new Regex("^BY$", RegexOptions.IgnoreCase);
        private static readonly Regex s_window =
            new Regex("^WINDOW$", RegexOptions.IgnoreCase);
        private static readonly Regex s_end =
            new Regex("^END$", RegexOptions.IgnoreCase);

        private static Func<Token, bool> IsToken(TokenTypes type, Regex regex)
        {
            return token => token?.Type == type && regex.IsMatch(token.Value);
        }

        public static bool IsAnd(Token token)
        {
            return IsToken(TokenTypes.RESERVED_NEWLINE, s_and).Invoke(token);
        }

        public static bool IsBetween(Token token)
        {
            return IsToken(TokenTypes.RESERVED, s_between).Invoke(token);
        }

        public static bool IsLimit(Token token)
        {
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, s_limit).Invoke(token);
        }

        public static bool IsSet(Token token)
        {
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, s_set).Invoke(token);
        }

        public static bool IsBy(Token token)
        {
            return IsToken(TokenTypes.RESERVED, s_by).Invoke(token);
        }

        public static bool IsWindow(Token token)
        {
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, s_window).Invoke(token);
        }

        public static bool IsEnd(Token token)
        {
            return IsToken(TokenTypes.CLOSE_PAREN, s_end).Invoke(token);
        }
    }
}
