using VerticalBlank.SqlFormatter.core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core
{
    public class Tokenizer
    {
        // private readonly Regex WHITESPACE_PATTERN;
        private readonly Regex NUMBER_PATTERN;
        private readonly Regex OPERATOR_PATTERN;

        private readonly Regex BLOCK_COMMENT_PATTERN;
        private readonly Regex LINE_COMMENT_PATTERN;

        private readonly Regex RESERVED_TOP_LEVEL_PATTERN;
        private readonly Regex RESERVED_TOP_LEVEL_NO_INDENT_PATTERN;
        private readonly Regex RESERVED_NEWLINE_PATTERN;
        private readonly Regex RESERVED_PLAIN_PATTERN;

        private readonly Regex WORD_PATTERN;
        private readonly Regex STRING_PATTERN;

        private readonly Regex OPEN_PAREN_PATTERN;
        private readonly Regex CLOSE_PAREN_PATTERN;

        private readonly Regex INDEXED_PLACEHOLDER_PATTERN;
        private readonly Regex IDENT_NAMED_PLACEHOLDER_PATTERN;
        private readonly Regex STRING_NAMED_PLACEHOLDER_PATTERN;

        /**
         * @param cfg {String[]} cfg.reservedWords Reserved words in SQL {String[]}
         *     cfg.reservedTopLevelWords Words that are set to new line separately {String[]}
         *     cfg.reservedNewlineWords Words that are set to newline {String[]} cfg.stringTypes String
         *     types to enable: "", "", ``, [], N"" {String[]} cfg.openParens Opening parentheses to
         *     enable, like (, [ {String[]} cfg.closeParens Closing parentheses to enable, like ), ]
         *     {String[]} cfg.indexedPlaceholderTypes Prefixes for indexed placeholders, like ? {String[]}
         *     cfg.namedPlaceholderTypes Prefixes for named placeholders, like @ and : {String[]}
         *     cfg.lineCommentTypes Line comments to enable, like # and -- {String[]} cfg.specialWordChars
         *     Special chars that can be found inside of words, like @ and #
         */
        public Tokenizer(DialectConfig cfg)
        {
            // this.WHITESPACE_PATTERN = Pattern.compile("^(\\s+)");
            NUMBER_PATTERN = new Regex(
                "^((-\\s*)?[0-9]+(\\.[0-9]+)?([eE]-?[0-9]+(\\.[0-9]+)?)?|0x[0-9a-fA-F]+|0b[01]+)\\b");

            OPERATOR_PATTERN = new Regex(
                RegexUtil.CreateOperatorRegex(
                    new JSLikeList<string>(new List<string>{ "<>", "<=", ">=" }).With(cfg.operators)));

            //        this.BLOCK_COMMENT_REGEX = /^(\/\*[^]*?(?:\*\/|$))/;
            BLOCK_COMMENT_PATTERN = new Regex("^(/\\*(?s).*?(?:\\*/|$))");
            LINE_COMMENT_PATTERN = new Regex(
                RegexUtil.CreateLineCommentRegex(new JSLikeList<string>(cfg.lineCommentTypes)));

            RESERVED_TOP_LEVEL_PATTERN =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedTopLevelWords)));
            RESERVED_TOP_LEVEL_NO_INDENT_PATTERN =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedTopLevelWordsNoIndent)));
            RESERVED_NEWLINE_PATTERN =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedNewlineWords)));
            RESERVED_PLAIN_PATTERN =
                new Regex(RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedWords)));

            WORD_PATTERN =
                new Regex(RegexUtil.CreateWordRegex(new JSLikeList<string>(cfg.specialWordChars)));
            STRING_PATTERN =
                new Regex(RegexUtil.CreateStringRegex(new JSLikeList<string>(cfg.stringTypes)));

            OPEN_PAREN_PATTERN =
                new Regex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.openParens)));
            CLOSE_PAREN_PATTERN =
                new Regex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.closeParens)));

            INDEXED_PLACEHOLDER_PATTERN =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.indexedPlaceholderTypes), "[0-9]*");
            IDENT_NAMED_PLACEHOLDER_PATTERN =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.namedPlaceholderTypes), "[a-zA-Z0-9._$]+");
            STRING_NAMED_PLACEHOLDER_PATTERN =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.namedPlaceholderTypes),
                    RegexUtil.CreateStringPattern(new JSLikeList<string>(cfg.stringTypes)));
        }

        /**
         * Takes a SQL string and breaks it into tokens. Each token is an object with type and value.
         *
         * @param input input The SQL string
         * @return {Object[]} tokens An array of tokens.
         */
        public JSLikeList<Token> Tokenize(string input)
        {
            List<Token> tokens = new List<Token>();
            Token token = null;

            // Keep processing the string until it is empty
            while (!string.IsNullOrEmpty(input))
            {
                // grab any preceding whitespace
                string[] findBeforeWhitespace = FindBeforeWhitespace(input);
                string whitespaceBefore = findBeforeWhitespace[0];
                input = findBeforeWhitespace[1];

                if (!string.IsNullOrEmpty(input))
                {
                    // Get the next token and the token type
                    token = GetNextToken(input, token);
                    // Advance the string
                    input = input.Substring(token.value.Length);

                    tokens.Add(token.WithWhitespaceBefore(whitespaceBefore));
                }
            }
            return new JSLikeList<Token>(tokens);
        }

        private string[] FindBeforeWhitespace(string input)
        {
            int index = 0;
            char[] chars = input.ToCharArray();
            int beforeLength = chars.Length;
            while (index != beforeLength && char.IsWhiteSpace(chars[index]))
            {
                index++;
            }
            return new string[] {
                new string(chars, 0, index), new string(chars, index, beforeLength - index)
            };
        }

        // private String GetWhitespace(String input) {
        //   String firstMatch = GetFirstMatch(input, WHITESPACE_PATTERN);
        //   return firstMatch != null ? firstMatch : "";
        // }

        private Token GetNextToken(string input, Token previousToken)
        {
            return Util.FirstNotnull(
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
            return Util.FirstNotnull(
                () => GetLineCommentToken(input), () => GetBlockCommentToken(input));
        }

        private Token GetLineCommentToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.LINE_COMMENT, LINE_COMMENT_PATTERN);
        }

        private Token GetBlockCommentToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.BLOCK_COMMENT, BLOCK_COMMENT_PATTERN);
        }

        private Token GetStringToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.STRING, STRING_PATTERN);
        }

        private Token GetOpenParenToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.OPEN_PAREN, OPEN_PAREN_PATTERN);
        }

        private Token GetCloseParenToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.CLOSE_PAREN, CLOSE_PAREN_PATTERN);
        }

        private Token GetPlaceholderToken(string input)
        {
            return Util.FirstNotnull(
                () => GetIdentNamedPlaceholderToken(input),
                () => GetStringNamedPlaceholderToken(input),
                () => GetIndexedPlaceholderToken(input));
        }

        private Token GetIdentNamedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input, IDENT_NAMED_PLACEHOLDER_PATTERN, v => v.Substring(1));
        }

        private Token GetStringNamedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input,
                STRING_NAMED_PLACEHOLDER_PATTERN,
                v => GetEscapedPlaceholderKey(
                    v.Substring(2, v.Length - 3), v.Substring(v.Length - 1)));
        }

        private Token GetIndexedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input, INDEXED_PLACEHOLDER_PATTERN, v => v.Substring(1));
        }

        private Token GetPlaceholderTokenWithKey(
            string input, Regex regex, Func<string, string> parseKey)
        {
            Token token = GetTokenOnFirstMatch(input, TokenTypes.PLACEHOLDER, regex);
            if (token != null)
            {
                return token.WithKey(parseKey.Invoke(token.value));
            }
            return token;
        }

        private string GetEscapedPlaceholderKey(string key, string quoteChar)
        {
            return key.Replace(RegexUtil.EscapeRegExp("\\") + quoteChar, quoteChar);
        }

        // Decimal, binary, or hex numbers
        private Token GetNumberToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.NUMBER, NUMBER_PATTERN);
        }

        // Punctuation and symbols
        private Token GetOperatorToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.OPERATOR, OPERATOR_PATTERN);
        }

        private Token GetReservedWordToken(string input, Token previousToken)
        {
            // A reserved word cannot be preceded by a "."
            // this makes it so in "mytable.from", "from" is not considered a reserved word
            if (previousToken != null && previousToken.value != null && previousToken.value.Equals("."))
            {
                return null;
            }
            return Util.FirstNotnull(
                () => GetToplevelReservedToken(input),
                () => GetNewlineReservedToken(input),
                () => GetTopLevelReservedTokenNoIndent(input),
                () => GetPlainReservedToken(input));
        }

        private Token GetToplevelReservedToken(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_TOP_LEVEL, RESERVED_TOP_LEVEL_PATTERN);
        }

        private Token GetNewlineReservedToken(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_NEWLINE, RESERVED_NEWLINE_PATTERN);
        }

        private Token GetTopLevelReservedTokenNoIndent(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT, RESERVED_TOP_LEVEL_NO_INDENT_PATTERN);
        }

        private Token GetPlainReservedToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.RESERVED, RESERVED_PLAIN_PATTERN);
        }

        private Token GetWordToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.WORD, WORD_PATTERN);
        }

        private static string GetFirstMatch(string input, Regex regex)
        {
            if (regex == null)
            {
                return null;
            }

            Match matcher = regex.Match(input);
            if (matcher.Success)
            {
                return matcher.Value;
            }
            else
            {
                return null;
            }
        }

        private Token GetTokenOnFirstMatch(string input, TokenTypes type, Regex regex)
        {
            string firstMatch = GetFirstMatch(input, regex);

            if (firstMatch != null)
            {
                return new Token(type, firstMatch);
            }
            else
            {
                return null;
            }
        }
    }
}
