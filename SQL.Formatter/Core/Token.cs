using System.Text.RegularExpressions;

namespace SQL.Formatter.Core
{
    public class Token
    {
        public readonly TokenTypes Type;
        public readonly string Value;
        public readonly string Regex;
        public readonly string Key;

        private readonly string _input;
        public readonly int WhitespaceStart;
        public readonly int WhitespaceLength;

        public string WhitespaceBefore =>
            (_input != null && WhitespaceLength > 0)
            ? _input.Substring(WhitespaceStart, WhitespaceLength)
            : string.Empty;

        public Token(
            TokenTypes type,
            string value,
            string regex = null,
            string key = null,
            string input = null,
            int wsStart = 0,
            int wsLen = 0)
        {
            Type = type;
            Value = value;
            Regex = regex;
            Key = key;
            _input = input;
            WhitespaceStart = wsStart;
            WhitespaceLength = wsLen;
        }

        public Token WithWhitespace(string input, int start, int length)
        {
            return new Token(Type, Value, Regex, Key, input, start, length);
        }

        public Token WithKey(string key)
        {
            return new Token(Type, Value, Regex, key, _input, WhitespaceStart, WhitespaceLength);
        }

        public override string ToString()
        {
            return $"type: {Type}, value: [{Value}], regex: /{Regex}/, key: {Key}";
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

        private static bool IsToken(Token token, TokenTypes type, Regex regex)
        {
            return token != null && token.Type == type && regex.IsMatch(token.Value);
        }

        public static bool IsAnd(Token token) =>
            IsToken(token, TokenTypes.RESERVED_NEWLINE, s_and);

        public static bool IsBetween(Token token) =>
            IsToken(token, TokenTypes.RESERVED, s_between);

        public static bool IsLimit(Token token) =>
            IsToken(token, TokenTypes.RESERVED_TOP_LEVEL, s_limit);

        public static bool IsSet(Token token) =>
            IsToken(token, TokenTypes.RESERVED_TOP_LEVEL, s_set);

        public static bool IsBy(Token token) =>
            IsToken(token, TokenTypes.RESERVED, s_by);

        public static bool IsWindow(Token token) =>
            IsToken(token, TokenTypes.RESERVED_TOP_LEVEL, s_window);

        public static bool IsEnd(Token token) =>
            IsToken(token, TokenTypes.CLOSE_PAREN, s_end);
    }
}
