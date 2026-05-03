using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SQL.Formatter.Core.Util;

namespace SQL.Formatter.Core
{
    public class Tokenizer
    {
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
            _numberPattern = new Regex(
                "^((-\\s*)?[0-9]+(\\.[0-9]+)?([eE]-?[0-9]+(\\.[0-9]+)?)?|0x[0-9a-fA-F]+|0b[01]+)\\b");

            _operatorPattern = new Regex(
                RegexUtil.CreateOperatorRegex(
                    new JSLikeList<string>(new List<string> { "<>", "<=", ">=" }).With(cfg.Operators)));

            _blockCommentPattern = new Regex("^(/\\*(?s).*?(?:\\*/|$))");
            _lineCommentPattern = new Regex(
                RegexUtil.CreateLineCommentRegex(new JSLikeList<string>(cfg.LineCommentTypes)));

            _reservedTopLevelPattern =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedTopLevelWords)));
            _reservedTopLevelNoIndentPattern =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedTopLevelWordsNoIndent)));
            _reservedNewLinePattern =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedNewlineWords)));
            _reservedPlainPattern =
                new Regex(RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.ReservedWords)));

            _wordPattern =
                new Regex(RegexUtil.CreateWordRegex(new JSLikeList<string>(cfg.SpecialWordChars)));
            _stringPattern =
                new Regex(RegexUtil.CreateStringRegex(new JSLikeList<string>(cfg.StringTypes)));

            _openParenPattern =
                new Regex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.OpenParens)));
            _closeParenPattern =
                new Regex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.CloseParens)));

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
            var tokens = new List<Token>();
            Token previousToken = default;
            var currentIndex = 0;

            while (currentIndex < input.Length)
            {
                var (wsOffset, wsLen) = SkipWhitespace(input, currentIndex);
                currentIndex += wsLen;

                if (currentIndex >= input.Length)
                {
                    break;
                }

                var token = GetNextToken(input, currentIndex, previousToken);

                if (token != null)
                {
                    tokens.Add(token.WithWhitespace(input, wsOffset, wsLen));
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

        private Token GetNextToken(string input, int offset, Token previousToken)
        {
            return Utils.FirstNotnull(
                () => GetCommentToken(input, offset),
                () => GetStringToken(input, offset),
                () => GetOpenParenToken(input, offset),
                () => GetCloseParenToken(input, offset),
                () => GetPlaceholderToken(input, offset),
                () => GetNumberToken(input, offset),
                () => GetReservedWordToken(input, offset, previousToken),
                () => GetWordToken(input, offset),
                () => GetOperatorToken(input, offset));
        }

        private Token GetCommentToken(string input, int offset)
        {
            return Utils.FirstNotnull(
                () => GetTokenOnFirstMatch(input, offset, TokenTypes.LINE_COMMENT, _lineCommentPattern),
                () => GetTokenOnFirstMatch(input, offset, TokenTypes.BLOCK_COMMENT, _blockCommentPattern));
        }

        private Token GetStringToken(string input, int offset) =>
            GetTokenOnFirstMatch(input, offset, TokenTypes.STRING, _stringPattern);

        private Token GetOpenParenToken(string input, int offset) =>
            GetTokenOnFirstMatch(input, offset, TokenTypes.OPEN_PAREN, _openParenPattern);

        private Token GetCloseParenToken(string input, int offset) =>
            GetTokenOnFirstMatch(input, offset, TokenTypes.CLOSE_PAREN, _closeParenPattern);

        private Token GetPlaceholderToken(string input, int offset)
        {
            return Utils.FirstNotnull(
                () => GetPlaceholderTokenWithKey(input, offset, _indentNamedPlaceholderPattern, v => v.Substring(1)),
                () => GetPlaceholderTokenWithKey(input, offset, _stringNamedPlaceholderPattern, v =>
                {
                    return GetEscapedPlaceholderKey(v.Substring(2, v.Length - 3), v.Substring(v.Length - 1));
                }),
                () => GetPlaceholderTokenWithKey(input, offset, _indexedPlaceholderPattern, v => v.Substring(1)));
        }

        private static Token GetPlaceholderTokenWithKey(string input, int offset, Regex regex, Func<string, string> parseKey)
        {
            var token = GetTokenOnFirstMatch(input, offset, TokenTypes.PLACEHOLDER, regex);
            return token?.WithKey(parseKey.Invoke(token.Value));
        }

        private static string GetEscapedPlaceholderKey(string key, string quoteChar) =>
            key.Replace(RegexUtil.EscapeRegExp("\\") + quoteChar, quoteChar);

        private Token GetNumberToken(string input, int offset) =>
            GetTokenOnFirstMatch(input, offset, TokenTypes.NUMBER, _numberPattern);

        private Token GetOperatorToken(string input, int offset) =>
            GetTokenOnFirstMatch(input, offset, TokenTypes.OPERATOR, _operatorPattern);

        private Token GetReservedWordToken(string input, int offset, Token previousToken)
        {
            return previousToken?.Value == "."
                ? default
                : Utils.FirstNotnull(
                () => GetTokenOnFirstMatch(input, offset, TokenTypes.RESERVED_TOP_LEVEL, _reservedTopLevelPattern),
                () => GetTokenOnFirstMatch(input, offset, TokenTypes.RESERVED_NEWLINE, _reservedNewLinePattern),
                () => GetTokenOnFirstMatch(input, offset, TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT, _reservedTopLevelNoIndentPattern),
                () => GetTokenOnFirstMatch(input, offset, TokenTypes.RESERVED, _reservedPlainPattern));
        }

        private Token GetWordToken(string input, int offset) =>
            GetTokenOnFirstMatch(input, offset, TokenTypes.WORD, _wordPattern);

        private static Token GetTokenOnFirstMatch(string input, int offset, TokenTypes type, Regex regex)
        {
            if (regex == null)
            {
                return default;
            }

            var match = regex.Match(input, offset, input.Length - offset);

            if (match.Success && match.Index == offset)
            {
                return new Token(type, match.Value);
            }

            return default;
        }
    }
}
