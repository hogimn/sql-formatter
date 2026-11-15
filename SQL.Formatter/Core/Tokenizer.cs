using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SQL.Formatter.Core.Util;

namespace SQL.Formatter.Core
{
    public class Tokenizer
    {
        private static readonly Regex s_numberPattern = new Regex(
            "^((-\\s*)?[0-9]+(\\.[0-9]+)?([eE]-?[0-9]+(\\.[0-9]+)?)?|0x[0-9a-fA-F]+|0b[01]+)\\b");
        private static readonly Regex s_blockCommentPattern = new Regex("^(/\\*(?s).*?(?:\\*/|$))");
        private static readonly object s_regexCacheLock = new object();
        private static readonly Dictionary<string, Regex> s_regexCache = new Dictionary<string, Regex>();
        private static readonly Queue<string> s_regexCacheOrder = new Queue<string>();
        private const int RegexCacheLimit = 256;

        private readonly Regex _numberPattern;
        private readonly Regex _operatorPattern;

        private readonly Regex _blockCommentPattern;
        private readonly Regex _lineCommentPattern;

        private readonly Regex _reservedTopLevelPattern;
        private readonly Regex _reservedTopLevelNoIndentPattern;
        private readonly Regex _reservedNewLinePattern;
        private readonly Regex _reservedPlainPattern;

        private readonly Regex _wordPattern;
        private readonly Regex _stringPattern;

        private readonly Regex _openParenPattern;
        private readonly Regex _closeParenPattern;

        private readonly Regex _indexedPlaceholderPattern;
        private readonly Regex _indentNamedPlaceholderPattern;
        private readonly Regex _stringNamedPlaceholderPattern;

        public Tokenizer(DialectConfig cfg)
        {
            _numberPattern = s_numberPattern;

            _operatorPattern = GetCachedRegex(
                RegexUtil.CreateOperatorRegex(
                    new JSLikeList<string>(new List<string> { "<>", "<=", ">=" }).With(cfg.Operators)));

            _blockCommentPattern = s_blockCommentPattern;
            _lineCommentPattern = GetCachedRegex(
                RegexUtil.CreateLineCommentRegex(new JSLikeList<string>(cfg.LineCommentTypes)));

            _reservedTopLevelPattern =
                GetCachedRegex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedTopLevelWords)));
            _reservedTopLevelNoIndentPattern =
                GetCachedRegex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedTopLevelWordsNoIndent)));
            _reservedNewLinePattern =
                GetCachedRegex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedNewlineWords)));
            _reservedPlainPattern =
                GetCachedRegex(RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedWords)));

            _wordPattern =
                GetCachedRegex(RegexUtil.CreateWordRegex(new JSLikeList<string>(cfg.SpecialWordChars)));
            _stringPattern =
                GetCachedRegex(RegexUtil.CreateStringRegex(new JSLikeList<string>(cfg.StringTypes)));

            _openParenPattern =
                GetCachedRegex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.OpenParens)));
            _closeParenPattern =
                GetCachedRegex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.CloseParens)));

            _indexedPlaceholderPattern =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.IndexedPlaceholderTypes), "[0-9]*");
            _indentNamedPlaceholderPattern =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.NamedPlaceholderTypes), "[a-zA-Z0-9._$]+");
            _stringNamedPlaceholderPattern =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.NamedPlaceholderTypes),
                    RegexUtil.CreateStringPattern(new JSLikeList<string>(cfg.StringTypes)));
        }

        private (int offset, int length) SkipWhitespace(string input, int start)
        {
            var current = start;
            while (current < input.Length && char.IsWhiteSpace(input[current]))
            {
                current++;
            }

            return (start, current - start);
        }

        public JSLikeList<Token> Tokenize(string input)
        {
            var tokens = new List<Token>(Math.Min(input.Length / 4, 1024));
            Token previousToken = null;
            var currentIndex = 0;

            while (currentIndex < input.Length)
            {
                var (wsOffset, wsLen) = SkipWhitespace(input, currentIndex);
                currentIndex += wsLen;

                if (currentIndex >= input.Length)
                {
                    break;
                }

                var token = GetNextToken(input, currentIndex, previousToken, wsOffset, wsLen);

                if (token != null)
                {
                    tokens.Add(token);
                    currentIndex += token.Value.Length;
                    previousToken = token;
                }
                else
                {
                    currentIndex++;
                }
            }

            return new JSLikeList<Token>(tokens);
        }

        private Token GetNextToken(
            string input,
            int offset,
            Token previousToken,
            int whitespaceStart,
            int whitespaceLength)
        {
            var token = GetCommentToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetStringToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetOpenParenToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetCloseParenToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetPlaceholderToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetNumberToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetReservedWordToken(input, offset, previousToken, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetWordToken(input, offset, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            return GetOperatorToken(input, offset, whitespaceStart, whitespaceLength);
        }

        private Token GetCommentToken(string input, int offset, int whitespaceStart, int whitespaceLength)
        {
            var token = GetTokenOnFirstMatch(
                input, offset, TokenTypes.LINE_COMMENT, _lineCommentPattern, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            return GetTokenOnFirstMatch(
                input, offset, TokenTypes.BLOCK_COMMENT, _blockCommentPattern, whitespaceStart, whitespaceLength);
        }

        private Token GetReservedWordToken(
            string input,
            int offset,
            Token previousToken,
            int whitespaceStart,
            int whitespaceLength)
        {
            if (previousToken?.Value == ".")
            {
                return null;
            }

            var token = GetTokenOnFirstMatch(
                input, offset, TokenTypes.RESERVED_TOP_LEVEL, _reservedTopLevelPattern, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetTokenOnFirstMatch(
                input, offset, TokenTypes.RESERVED_NEWLINE, _reservedNewLinePattern, whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            token = GetTokenOnFirstMatch(
                input, offset, TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT, _reservedTopLevelNoIndentPattern,
                whitespaceStart, whitespaceLength);
            if (token != null)
            {
                return token;
            }

            return GetTokenOnFirstMatch(
                input, offset, TokenTypes.RESERVED, _reservedPlainPattern, whitespaceStart, whitespaceLength);
        }

        private Token GetStringToken(string input, int offset, int whitespaceStart, int whitespaceLength) =>
            GetTokenOnFirstMatch(
                input, offset, TokenTypes.STRING, _stringPattern, whitespaceStart, whitespaceLength);

        private Token GetOpenParenToken(string input, int offset, int whitespaceStart, int whitespaceLength) =>
            GetTokenOnFirstMatch(
                input, offset, TokenTypes.OPEN_PAREN, _openParenPattern, whitespaceStart, whitespaceLength);

        private Token GetCloseParenToken(string input, int offset, int whitespaceStart, int whitespaceLength) =>
            GetTokenOnFirstMatch(
                input, offset, TokenTypes.CLOSE_PAREN, _closeParenPattern, whitespaceStart, whitespaceLength);

        private Token GetPlaceholderToken(string input, int offset, int whitespaceStart, int whitespaceLength)
        {
            var match = GetMatchOnFirstMatch(input, offset, _indentNamedPlaceholderPattern);
            if (match != null)
            {
                return new Token(TokenTypes.PLACEHOLDER, match.Value, key: match.Value.Substring(1),
                    input: input, wsStart: whitespaceStart, wsLen: whitespaceLength);
            }

            match = GetMatchOnFirstMatch(input, offset, _stringNamedPlaceholderPattern);
            if (match != null)
            {
                var v = match.Value;
                var key = GetEscapedPlaceholderKey(v.Substring(2, v.Length - 3), v.Substring(v.Length - 1));
                return new Token(TokenTypes.PLACEHOLDER, v, key: key,
                    input: input, wsStart: whitespaceStart, wsLen: whitespaceLength);
            }

            match = GetMatchOnFirstMatch(input, offset, _indexedPlaceholderPattern);
            if (match != null)
            {
                return new Token(TokenTypes.PLACEHOLDER, match.Value, key: match.Value.Substring(1),
                    input: input, wsStart: whitespaceStart, wsLen: whitespaceLength);
            }

            return null;
        }

        private static string GetEscapedPlaceholderKey(string key, string quoteChar) =>
            key.Replace(RegexUtil.EscapeRegExp("\\") + quoteChar, quoteChar);

        private Token GetNumberToken(string input, int offset, int whitespaceStart, int whitespaceLength) =>
            GetTokenOnFirstMatch(
                input, offset, TokenTypes.NUMBER, _numberPattern, whitespaceStart, whitespaceLength);

        private Token GetOperatorToken(string input, int offset, int whitespaceStart, int whitespaceLength) =>
            GetTokenOnFirstMatch(
                input, offset, TokenTypes.OPERATOR, _operatorPattern, whitespaceStart, whitespaceLength);

        private Token GetWordToken(string input, int offset, int whitespaceStart, int whitespaceLength) =>
            GetTokenOnFirstMatch(
                input, offset, TokenTypes.WORD, _wordPattern, whitespaceStart, whitespaceLength);

        private static Token GetTokenOnFirstMatch(
            string input,
            int offset,
            TokenTypes type,
            Regex regex,
            int whitespaceStart,
            int whitespaceLength)
        {
            if (regex == null)
            {
                return null;
            }

            var match = GetMatchOnFirstMatch(input, offset, regex);

            if (match != null)
            {
                return new Token(type, match.Value, input: input, wsStart: whitespaceStart, wsLen: whitespaceLength);
            }

            return null;
        }

        private static Match GetMatchOnFirstMatch(string input, int offset, Regex regex)
        {
            if (regex == null)
            {
                return null;
            }

            var match = regex.Match(input, offset, input.Length - offset);
            return match.Success && match.Index == offset ? match : null;
        }

        private static Regex GetCachedRegex(string pattern)
        {
            lock (s_regexCacheLock)
            {
                if (s_regexCache.TryGetValue(pattern, out var cached))
                {
                    return cached;
                }
            }

            var regex = new Regex(pattern);

            lock (s_regexCacheLock)
            {
                if (s_regexCache.TryGetValue(pattern, out var cached))
                {
                    return cached;
                }

                if (s_regexCache.Count >= RegexCacheLimit)
                {
                    s_regexCache.Remove(s_regexCacheOrder.Dequeue());
                }

                s_regexCache.Add(pattern, regex);
                s_regexCacheOrder.Enqueue(pattern);
                return regex;
            }
        }
    }
}
