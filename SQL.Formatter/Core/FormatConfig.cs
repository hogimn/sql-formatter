using System;
using System.Collections.Generic;

namespace SQL.Formatter.Core
{
    public class FormatConfig
    {
        public static readonly string DefaultIndent = "  ";
        public static readonly int DefaultColumnMaxLength = 50;

        public readonly string Indent;
        public readonly int MaxColumnLength;
        public readonly Params Parameters;
        [Obsolete("Use Case instead of Uppercase")]
        public readonly bool Uppercase;
        public readonly CaseTypes Case;
        public readonly int LinesBetweenQueries;
        public readonly bool SkipWhitespaceNearBlockParentheses;

        [Obsolete("Use the constructor with caseType instead of uppercase")]
        public FormatConfig(
            string indent,
            int maxColumnLength,
            Params parameters,
            bool uppercase,
            int linesBetweenQueries,
            bool skipWhitespaceNearBlockParentheses)
            : this(
                  indent,
                  maxColumnLength,
                  parameters,
                  uppercase ? CaseTypes.UPPER : CaseTypes.NONE,
                  linesBetweenQueries,
                  skipWhitespaceNearBlockParentheses)
        {
        }

        public FormatConfig(
            string indent,
            int maxColumnLength,
            Params parameters,
            CaseTypes caseType,
            int linesBetweenQueries,
            bool skipWhitespaceNearBlockParentheses)
        {
            Indent = indent;
            MaxColumnLength = maxColumnLength;
            Parameters = parameters == null ? Params.Empty : parameters;
            Case = caseType;
#pragma warning disable CS0618
            Uppercase = caseType == CaseTypes.UPPER;
#pragma warning restore CS0618
            LinesBetweenQueries = linesBetweenQueries;
            SkipWhitespaceNearBlockParentheses = skipWhitespaceNearBlockParentheses;
        }

        public static FormatConfigBuilder Builder()
        {
            return new FormatConfigBuilder();
        }

        public class FormatConfigBuilder
        {
            private string _indent = DefaultIndent;
            private int _maxColumnLength = DefaultColumnMaxLength;
            private Params _parameters;
            private CaseTypes _case;
            private int _linesBetweenQueries;
            private bool _skipWhitespaceNearBlockParentheses;

            public FormatConfigBuilder()
            {
            }

            public FormatConfigBuilder Indent(string indent)
            {
                _indent = indent;
                return this;
            }

            public FormatConfigBuilder MaxColumnLength(int maxColumnLength)
            {
                _maxColumnLength = maxColumnLength;
                return this;
            }

            public FormatConfigBuilder Params(Params parameters)
            {
                _parameters = parameters;
                return this;
            }

            public FormatConfigBuilder Params<T>(Dictionary<string, T> parameters)
            {
                return Params(Core.Params.Of(parameters));
            }

            public FormatConfigBuilder Params<T>(List<T> parameters)
            {
                return Params(Core.Params.Of(parameters));
            }

            [Obsolete("Use Case instead of Uppercase")]
            public FormatConfigBuilder Uppercase(bool uppercase)
            {
                _case = uppercase ? CaseTypes.UPPER : CaseTypes.NONE;

                return this;
            }

            public FormatConfigBuilder Case(CaseTypes caseType)
            {
                _case = caseType;
                return this;
            }

            public FormatConfigBuilder LinesBetweenQueries(int linesBetweenQueries)
            {
                _linesBetweenQueries = linesBetweenQueries;
                return this;
            }

            public FormatConfigBuilder SkipWhitespaceNearBlockParentheses(bool skipWhitespaceNearBlockParentheses)
            {
                _skipWhitespaceNearBlockParentheses = skipWhitespaceNearBlockParentheses;
                return this;
            }

            public FormatConfig Build()
            {
                return new FormatConfig(
                    _indent,
                    _maxColumnLength,
                    _parameters,
                    _case,
                    _linesBetweenQueries,
                    _skipWhitespaceNearBlockParentheses);
            }
        }
    }
}
