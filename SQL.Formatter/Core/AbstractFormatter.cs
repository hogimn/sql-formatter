using System;
using System.Collections.Generic;
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
            var formattedQuery = GetFormattedQueryFromTokens();

            return formattedQuery.Trim();
        }

        private string GetFormattedQueryFromTokens()
        {
            var formattedQuery = string.Empty;

            var index = -1;
            foreach (Token t in _tokens)
            {
                _index = ++index;

                var token = TokenOverride(t);

                if (token.Type == TokenTypes.LINE_COMMENT)
                {
                    formattedQuery = FormatLineComment(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.BLOCK_COMMENT)
                {
                    formattedQuery = FormatBlockComment(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.RESERVED_TOP_LEVEL)
                {
                    formattedQuery = FormatToplevelReservedWord(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT)
                {
                    formattedQuery = FormatTopLevelReservedWordNoIndent(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.RESERVED_NEWLINE)
                {
                    formattedQuery = FormatNewlineReservedWord(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.RESERVED)
                {
                    formattedQuery = FormatWithSpaces(token, formattedQuery);
                    _previousReservedToken = token;
                }
                else if (token.Type == TokenTypes.OPEN_PAREN)
                {
                    formattedQuery = FormatOpeningParentheses(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.CLOSE_PAREN)
                {
                    formattedQuery = FormatClosingParentheses(token, formattedQuery);
                }
                else if (token.Type == TokenTypes.PLACEHOLDER)
                {
                    formattedQuery = FormatPlaceholder(token, formattedQuery);
                }
                else if (token.Value.Equals(","))
                {
                    formattedQuery = FormatComma(token, formattedQuery);
                }
                else if (token.Value.Equals(":"))
                {
                    formattedQuery = FormatWithSpaceAfter(token, formattedQuery);
                }
                else if (token.Value.Equals("."))
                {
                    formattedQuery = FormatWithoutSpaces(token, formattedQuery);
                }
                else if (token.Value.Equals(";"))
                {
                    formattedQuery = FormatQuerySeparator(token, formattedQuery);
                }
                else
                {
                    formattedQuery = FormatWithSpaces(token, formattedQuery);
                }
            }

            return formattedQuery;
        }

        protected virtual string FormatLineComment(Token token, string query)
        {
            return AddNewline(query + Show(token));
        }

        protected virtual string FormatBlockComment(Token token, string query)
        {
            return AddNewline(AddNewline(query) + IndentComment(token.Value));
        }

        protected virtual string IndentComment(string comment)
        {
            return comment.Replace("\n", "\n" + _indentation.GetIndent());
        }

        protected virtual string FormatTopLevelReservedWordNoIndent(Token token, string query)
        {
            _indentation.DecreaseTopLevel();
            query = AddNewline(query) + EqualizeWhitespace(Show(token));
            return AddNewline(query);
        }

        protected virtual string FormatToplevelReservedWord(Token token, string query)
        {
            _indentation.DecreaseTopLevel();

            query = AddNewline(query);

            _indentation.IncreaseTopLevel();

            query += EqualizeWhitespace(Show(token));
            return AddNewline(query);
        }

        protected virtual string FormatNewlineReservedWord(Token token, string query)
        {
            if (Token.IsAnd(token) && Token.IsBetween(TokenLookBehind(2)))
            {
                return FormatWithSpaces(token, query);
            }

            return AddNewline(query) + EqualizeWhitespace(Show(token)) + " ";
        }

        protected static string EqualizeWhitespace(string str)
        {
            return Regex.Replace(str, @"\s+", " ");
        }

        private static readonly HashSet<TokenTypes> s_preserveWhitespaceFor =
            new HashSet<TokenTypes> {
                TokenTypes.OPEN_PAREN,
                TokenTypes.LINE_COMMENT,
                TokenTypes.OPERATOR,
                TokenTypes.RESERVED_NEWLINE};

        protected virtual string FormatOpeningParentheses(Token token, string query)
        {
            if (string.IsNullOrEmpty(token.WhitespaceBefore)
                && (TokenLookBehind() == default || !s_preserveWhitespaceFor.Contains(TokenLookBehind().Type)))
            {
                query = query.TrimEnd();
            }

            query += Show(token);

            _inlineBlock.BeginIfPossible(_tokens, _index);

            if (!_inlineBlock.IsActive())
            {
                _indentation.IncreaseBlockLevel();
                if (!_cfg.SkipWhitespaceNearBlockParentheses)
                {
                    query = AddNewline(query);
                }
            }

            return query;
        }

        protected virtual string FormatClosingParentheses(Token token, string query)
        {
            if (_inlineBlock.IsActive())
            {
                _inlineBlock.End();
                return FormatWithSpaceAfter(token, query);
            }
            else
            {
                _indentation.DecreaseBlockLevel();

                if (!_cfg.SkipWhitespaceNearBlockParentheses)
                {
                    return FormatWithSpaces(token, AddNewline(query));
                }

                return FormatWithoutSpaces(token, query);
            }
        }

        protected virtual string FormatPlaceholder(Token token, string query)
        {
            return query + _parameters.Get(token) + " ";
        }

        protected virtual string FormatComma(Token token, string query)
        {
            query = query.TrimEnd() + Show(token) + " ";
            return _inlineBlock.IsActive() || Token.IsLimit(_previousReservedToken) ? query : AddNewline(query);
        }

        protected virtual string FormatWithSpaceAfter(Token token, string query)
        {
            return query.TrimEnd() + Show(token) + " ";
        }

        protected virtual string FormatWithoutSpaces(Token token, string query)
        {
            return query.TrimEnd() + Show(token);
        }

        protected virtual string FormatWithSpaces(Token token, string query)
        {
            return query + Show(token) + " ";
        }

        protected virtual string FormatQuerySeparator(Token token, string query)
        {
            _indentation.ResetIndentation();
            return query.TrimEnd()
                + Show(token)
                + Utils.Repeat("\n", _cfg.LinesBetweenQueries == default ? 1 : _cfg.LinesBetweenQueries);
        }

        protected virtual string Show(Token token)
        {
            if (_cfg.Uppercase
                && (token.Type == TokenTypes.RESERVED
                    || token.Type == TokenTypes.RESERVED_TOP_LEVEL
                    || token.Type == TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT
                    || token.Type == TokenTypes.RESERVED_NEWLINE
                    || token.Type == TokenTypes.OPEN_PAREN
                    || token.Type == TokenTypes.CLOSE_PAREN))
            {
                return token.Value.ToUpper();
            }

            return token.Value;
        }

        protected virtual string AddNewline(string query)
        {
            query = query.TrimEnd();
            if (!query.EndsWith("\n"))
            {
                query += "\n";
            }

            return query + _indentation.GetIndent();
        }

        protected Token TokenLookBehind()
        {
            return TokenLookBehind(1);
        }

        protected Token TokenLookBehind(int n)
        {
            return _tokens.Get(_index - n);
        }

        protected Token TokenLookAhead()
        {
            return TokenLookAhead(1);
        }

        protected Token TokenLookAhead(int n)
        {
            return _tokens.Get(_index + n);

        }

        public virtual DialectConfig DoDialectConfig()
        {
            return _doDialectConfigFunc.Invoke();
        }

        public Func<DialectConfig> _doDialectConfigFunc;
    }
}
