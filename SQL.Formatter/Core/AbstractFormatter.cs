using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SQL.Formatter.Core.Util;
using SQL.Formatter.Language;

namespace SQL.Formatter.Core
{
    public class AbstractFormatter : IDialectConfigurator
    {
        private readonly FormatConfig _cfg;
        private readonly Indentation _indentation;
        private readonly InlineBlock _inlineBlock;
        private readonly Params _parameters;
        protected Token _previousReservedToken;
        private JSLikeList<Token> _tokens;
        private int _index;

        private static readonly Regex s_whitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        private static readonly HashSet<TokenTypes> s_preserveWhitespaceFor =
            new HashSet<TokenTypes> {
                TokenTypes.OPEN_PAREN,
                TokenTypes.LINE_COMMENT,
                TokenTypes.OPERATOR,
                TokenTypes.RESERVED_NEWLINE
            };

        public Func<DialectConfig> _doDialectConfigFunc;

        public AbstractFormatter(FormatConfig cfg)
        {
            _cfg = cfg;
            _indentation = new Indentation(cfg.Indent);
            _inlineBlock = new InlineBlock(cfg.MaxColumnLength);
            _parameters = cfg.Parameters;
            _previousReservedToken = null;
            _index = 0;
        }

        protected Indentation Indentation => _indentation;

        public Tokenizer Tokenizer()
        {
            return new Tokenizer(DoDialectConfig());
        }

        protected virtual Token TokenOverride(Token token)
        {
            return token;
        }

        public string Format(string query)
        {
            _tokens = Tokenizer().Tokenize(query);
            return GetFormattedQueryFromTokens().Trim();
        }

        private string GetFormattedQueryFromTokens()
        {
            var formattedQuery = new StringBuilder(1024);

            var index = -1;
            foreach (Token t in _tokens)
            {
                _index = ++index;
                var token = TokenOverride(t);

                if (token.Type == TokenTypes.LINE_COMMENT)
                {
                    FormatLineComment(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.BLOCK_COMMENT)
                {
                    FormatBlockComment(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.RESERVED_TOP_LEVEL)
                {
                    FormatToplevelReservedWord(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT)
                {
                    FormatTopLevelReservedWordNoIndent(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.RESERVED_NEWLINE)
                {
                    FormatNewlineReservedWord(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.RESERVED)
                {
                    FormatWithSpaces(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.OPEN_PAREN)
                {
                    FormatOpeningParentheses(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.CLOSE_PAREN)
                {
                    FormatClosingParentheses(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.PLACEHOLDER)
                {
                    FormatPlaceholder(token, formattedQuery);
                }
                else if (token.Value.Equals(","))
                {
                    FormatComma(token, formattedQuery);
                }
                else if (token.Value.Equals(":"))
                {
                    FormatWithSpaceAfter(token, formattedQuery);
                }
                else if (token.Value.Equals("."))
                {
                    FormatWithoutSpaces(token, formattedQuery);
                }
                else if (token.Value.Equals(";"))
                {
                    FormatQuerySeparator(token, formattedQuery);
                }
                else
                {
                    FormatWithSpaces(token, formattedQuery);
                }
            }

            return formattedQuery.ToString();
        }

        protected virtual void FormatLineComment(Token token, StringBuilder query)
        {
            query.Append(Show(token));
            AddNewline(query);
        }

        protected virtual void FormatBlockComment(Token token, StringBuilder query)
        {
            AddNewline(query);
            query.Append(IndentComment(token.Value));
            AddNewline(query);
        }

        protected virtual string IndentComment(string comment)
        {
            return comment.Replace("\n", "\n" + _indentation.GetIndent());
        }

        protected virtual void FormatTopLevelReservedWordNoIndent(Token token, StringBuilder query)
        {
            _indentation.DecreaseTopLevel();
            AddNewline(query);
            query.Append(EqualizeWhitespace(Show(token)));
            AddNewline(query);
        }

        protected virtual void FormatToplevelReservedWord(Token token, StringBuilder query)
        {
            _indentation.DecreaseTopLevel();
            AddNewline(query);
            _indentation.IncreaseTopLevel();

            query.Append(EqualizeWhitespace(Show(token)));
            AddNewline(query);
        }

        protected virtual void FormatNewlineReservedWord(Token token, StringBuilder query)
        {
            if (Token.IsAnd(token) && Token.IsBetween(TokenLookBehind(2)))
            {
                FormatWithSpaces(token, query);
                return;
            }

            AddNewline(query);
            query.Append(EqualizeWhitespace(Show(token))).Append(" ");
        }

        protected static string EqualizeWhitespace(string str)
        {
            return s_whitespaceRegex.Replace(str, " ");
        }

        protected virtual void FormatOpeningParentheses(Token token, StringBuilder query)
        {
            if (string.IsNullOrEmpty(token.WhitespaceBefore)
                && (TokenLookBehind() == default || !s_preserveWhitespaceFor.Contains(TokenLookBehind().Type)))
            {
                TrimEnd(query);
            }

            query.Append(Show(token));

            _inlineBlock.BeginIfPossible(_tokens, _index);

            if (!_inlineBlock.IsActive())
            {
                _indentation.IncreaseBlockLevel();
                if (!_cfg.SkipWhitespaceNearBlockParentheses)
                {
                    AddNewline(query);
                }
            }
        }

        protected virtual void FormatClosingParentheses(Token token, StringBuilder query)
        {
            if (_inlineBlock.IsActive())
            {
                _inlineBlock.End();
                FormatWithSpaceAfter(token, query);
            }
            else
            {
                _indentation.DecreaseBlockLevel();

                if (!_cfg.SkipWhitespaceNearBlockParentheses)
                {
                    AddNewline(query);
                    FormatWithSpaces(token, query);
                }
                else
                {
                    FormatWithoutSpaces(token, query);
                }
            }
        }

        protected virtual void FormatPlaceholder(Token token, StringBuilder query)
        {
            query.Append(_parameters.Get(token)).Append(" ");
        }

        protected virtual void FormatComma(Token token, StringBuilder query)
        {
            TrimEnd(query);
            query.Append(Show(token)).Append(" ");

            if (!_inlineBlock.IsActive() && !Token.IsLimit(_previousReservedToken))
            {
                AddNewline(query);
            }
        }

        protected virtual void FormatWithSpaceAfter(Token token, StringBuilder query)
        {
            TrimEnd(query);
            query.Append(Show(token)).Append(" ");
        }

        protected virtual void FormatWithoutSpaces(Token token, StringBuilder query)
        {
            TrimEnd(query);
            query.Append(Show(token));
        }

        protected virtual void FormatWithSpaces(Token token, StringBuilder query)
        {
            query.Append(Show(token)).Append(" ");
        }

        protected virtual void FormatQuerySeparator(Token token, StringBuilder query)
        {
            _indentation.ResetIndentation();
            TrimEnd(query);
            query.Append(Show(token));

            var lines = _cfg.LinesBetweenQueries == default ? 1 : _cfg.LinesBetweenQueries;
            for (var i = 0; i < lines; i++)
            {
                query.Append('\n');
            }
        }

        protected virtual string Show(Token token)
        {
            if (_cfg.Case > CaseTypes.NONE
                && (token.Type == TokenTypes.RESERVED
                    || token.Type == TokenTypes.RESERVED_TOP_LEVEL
                    || token.Type == TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT
                    || token.Type == TokenTypes.RESERVED_NEWLINE
                    || token.Type == TokenTypes.OPEN_PAREN
                    || token.Type == TokenTypes.CLOSE_PAREN))
            {
                return _cfg.Case == CaseTypes.UPPER ? token.Value.ToUpper() : token.Value.ToLower();
            }

            return token.Value;
        }

        protected virtual void AddNewline(StringBuilder query)
        {
            TrimEnd(query);
            if (query.Length == 0 || query[query.Length - 1] != '\n')
            {
                query.Append('\n');
            }

            query.Append(_indentation.GetIndent());
        }

        protected void TrimEnd(StringBuilder sb)
        {
            while (sb.Length > 0 && char.IsWhiteSpace(sb[sb.Length - 1]))
            {
                sb.Length--;
            }
        }

        protected Token TokenLookBehind() => TokenLookBehind(1);
        protected Token TokenLookBehind(int n) => _tokens.Get(_index - n);
        protected Token TokenLookAhead() => TokenLookAhead(1);
        protected Token TokenLookAhead(int n) => _tokens.Get(_index + n);

        public virtual DialectConfig DoDialectConfig()
        {
            return _doDialectConfigFunc.Invoke();
        }
    }
}
