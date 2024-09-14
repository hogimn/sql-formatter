using System;
using System.Collections.Generic;
using System.Linq;
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

        public JSLikeList<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            Token token = null;

            while (!string.IsNullOrEmpty(input))
            {
                var findBeforeWhitespace = FindBeforeWhitespace(input);
                var whitespaceBefore = findBeforeWhitespace[0];
                input = findBeforeWhitespace[1];

                if (!string.IsNullOrEmpty(input))
                {
                    token = GetNextToken(input, token);
                    input = input[token.Value.Length..];
                    tokens.Add(token.WithWhitespaceBefore(whitespaceBefore));
                }
            }

            return new JSLikeList<Token>(tokens);
        }

        private static string[] FindBeforeWhitespace(string input)
        {
            var index = input.TakeWhile(char.IsWhiteSpace).Count();
            return new[] { input[..index], input[index..] };
        }

        private Token GetNextToken(string input, Token previousToken)
        {
            return Utils.FirstNotnull(
                () => GetCommentToken(input),
                () => GetStringToken(input),
                () => GetOpenParenToken(input),
                () => GetCloseParenToken(input),
                () => GetPlaceholderToken(input),
                () => GetNumberToken(input),
                () => GetReservedWordToken(input, previousToken),
                () => GetWordToken(input),
                () => GetOperatorToken(input));
        }

        private Token GetCommentToken(string input)
        {
            return Utils.FirstNotnull(
                () => GetLineCommentToken(input),
                () => GetBlockCommentToken(input));
        }

        private Token GetLineCommentToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.LINE_COMMENT, _lineCommentPattern);
        }

        private Token GetBlockCommentToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.BLOCK_COMMENT, _blockCommentPattern);
        }

        private Token GetStringToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.STRING, _stringPattern);
        }

        private Token GetOpenParenToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.OPEN_PAREN, _openParenPattern);
        }

        private Token GetCloseParenToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.CLOSE_PAREN, _closeParenPattern);
        }

        private Token GetPlaceholderToken(string input)
        {
            return Utils.FirstNotnull(
                () => GetIdentNamedPlaceholderToken(input),
                () => GetStringNamedPlaceholderToken(input),
                () => GetIndexedPlaceholderToken(input));
        }

        private Token GetIdentNamedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input, _indentNamedPlaceholderPattern, v => v[1..]);
        }

        private Token GetStringNamedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input,
                _stringNamedPlaceholderPattern,
                v => GetEscapedPlaceholderKey(
                    v[2..^1], v[^1..]));
        }

        private Token GetIndexedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input, _indexedPlaceholderPattern, v => v[1..]);
        }

        private static Token GetPlaceholderTokenWithKey(string input, Regex regex, Func<string, string> parseKey)
        {
            var token = GetTokenOnFirstMatch(input, TokenTypes.PLACEHOLDER, regex);
            return token?.WithKey(parseKey.Invoke(token.Value));
        }

        private static string GetEscapedPlaceholderKey(string key, string quoteChar)
        {
            return key.Replace(RegexUtil.EscapeRegExp("\\") + quoteChar, quoteChar);
        }

        private Token GetNumberToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.NUMBER, _numberPattern);
        }

        private Token GetOperatorToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.OPERATOR, _operatorPattern);
        }

        private Token GetReservedWordToken(string input, Token previousToken)
        {
            if (previousToken?.Value != null && previousToken.Value.Equals("."))
            {
                return null;
            }

            return Utils.FirstNotnull(
                () => GetToplevelReservedToken(input),
                () => GetNewlineReservedToken(input),
                () => GetTopLevelReservedTokenNoIndent(input),
                () => GetPlainReservedToken(input));
        }

        private Token GetToplevelReservedToken(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_TOP_LEVEL, _reservedTopLevelPattern);
        }

        private Token GetNewlineReservedToken(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_NEWLINE, _reservedNewLinePattern);
        }

        private Token GetTopLevelReservedTokenNoIndent(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT, _reservedTopLevelNoIndentPattern);
        }

        private Token GetPlainReservedToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.RESERVED, _reservedPlainPattern);
        }

        private Token GetWordToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.WORD, _wordPattern);
        }

        private static string GetFirstMatch(string input, Regex regex)
        {
            return regex?.Match(input).Value ?? string.Empty;
        }

        private static Token GetTokenOnFirstMatch(string input, TokenTypes type, Regex regex)
        {
            var match = GetFirstMatch(input, regex);
            return match.Equals(string.Empty) ? default : new Token(type, match);
        }
    }
}
