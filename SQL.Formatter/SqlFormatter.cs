using System;
using System.Collections.Generic;
using SQL.Formatter.Core;
using SQL.Formatter.Language;

namespace SQL.Formatter
{
    public class SqlFormatter
    {
        public static string Format(string query, FormatConfig cfg)
        {
            return Standard().Format(query, cfg);
        }

        public static string Format<T>(string query, string indent, List<T> parameters)
        {
            return Standard().Format(query, indent, parameters);
        }

        public static string Format<T>(string query, List<T> parameters)
        {
            return Standard().Format(query, parameters);
        }

        public static string Format<T>(string query, string indent, Dictionary<string, T> parameters)
        {
            return Standard().Format(query, indent, parameters);
        }

        public static string Format<T>(string query, Dictionary<string, T> parameters)
        {
            return Standard().Format(query, parameters);
        }

        public static string Format(string query, string indent)
        {
            return Standard().Format(query, indent);
        }

        public static string Format(string query)
        {
            return Standard().Format(query);
        }

        public static Formatter Extend(Func<DialectConfig, DialectConfig> sqlOperator)
        {
            return Standard().Extend(sqlOperator);
        }

        public static Formatter Standard()
        {
            return Of(Dialect.StandardSql);
        }

        public static Formatter Of(string name)
        {
            var dialect = Dialect.NameOf(name);
            return dialect == null ? throw new Exception("Unsupported SQL dialect: " + name) :
                new Formatter(dialect);
        }

        public static Formatter Of(Dialect dialect)
        {
            return new Formatter(dialect);
        }

        public class Formatter
        {

            private readonly Func<FormatConfig, AbstractFormatter> underlying;

            public Formatter(Func<FormatConfig, AbstractFormatter> underlying)
            {
                this.underlying = underlying;
            }

            public Formatter(Dialect dialect) : this(dialect.func) { }

            public string Format(string query, FormatConfig cfg)
            {
                return underlying.Invoke(cfg).Format(query);
            }

            public string Format<T>(string query, string indent, List<T> parameters)
            {
                return Format(query, FormatConfig.Builder().Indent(indent).Params(parameters).Build());
            }

            public string Format<T>(string query, List<T> parameters)
            {
                return Format(query, FormatConfig.Builder().Params(parameters).Build());
            }

            public string Format<T>(string query, string indent, Dictionary<string, T> parameters)
            {
                return Format(query, FormatConfig.Builder().Indent(indent).Params(parameters).Build());
            }

            public string Format<T>(string query, Dictionary<string, T> parameters)
            {
                return Format(query, FormatConfig.Builder().Params(parameters).Build());
            }

            public string Format(string query, string indent)
            {
                return Format(query, FormatConfig.Builder().Indent(indent).Build());
            }

            public string Format(string query)
            {
                return Format(query, FormatConfig.Builder().Build());
            }

            public Formatter Extend(Func<DialectConfig, DialectConfig> sqlOperator)
            {
                AbstractFormatter Func(FormatConfig config)
                {
                    var abstractFormatter = new AbstractFormatter(config)
                    {
                        doDialectConfigFunc = () => sqlOperator.Invoke(underlying.Invoke(config).DoDialectConfig())
                    };
                    return abstractFormatter;
                }

                return new Formatter(Func);
            }
        }
    }
}

