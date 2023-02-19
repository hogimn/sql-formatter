using System;
using System.Collections.Generic;
using SQL.Formatter.core;
using SQL.Formatter.languages;

namespace SQL.Formatter
{
    public class SqlFormatter
    {
        /**
         * FormatConfig whitespaces in a query to make it easier to read.
         *
         * @param query sql
         * @param cfg FormatConfig
         * @return Formatted query
         */
        public static string Format(string query, FormatConfig cfg)
        {
            return Standard().Format(query, cfg);
        }

        public static string Format<T>(string query, string Indent, List<T> Parameters)
        {
            return Standard().Format(query, Indent, Parameters);
        }

        public static string Format<T>(string query, List<T> Parameters)
        {
            return Standard().Format(query, Parameters);
        }

        public static string Format<T>(string query, string Indent, Dictionary<string, T> Parameters)
        {
            return Standard().Format(query, Indent, Parameters);
        }

        public static string Format<T>(string query, Dictionary<string, T> Parameters)
        {
            return Standard().Format(query, Parameters);
        }

        public static string Format(string query, string Indent)
        {
            return Standard().Format(query, Indent);
        }

        public static string Format(string query)
        {
            return Standard().Format(query);
        }

        public static Formatter Extend(Func<DialectConfig, DialectConfig> oper)
        {
            return Standard().Extend(oper);
        }

        public static Formatter Standard()
        {
            return Of(Dialect.StandardSql);
        }

        public static Formatter Of(string name)
        {
            Dialect dialect = Dialect.NameOf(name);
            if (dialect == null)
                throw new Exception("Unsupported SQL dialect: " + name);
            return new Formatter(dialect);
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

            public string Format<T>(string query, string Indent, List<T> Parameters)
            {
                return Format(query, FormatConfig.Builder().Indent(Indent).Params(Parameters).Build());
            }

            public string Format<T>(string query, List<T> Parameters)
            {
                return Format(query, FormatConfig.Builder().Params(Parameters).Build());
            }

            public string Format<T>(string query, string Indent, Dictionary<string, T> Parameters)
            {
                return Format(query, FormatConfig.Builder().Indent(Indent).Params(Parameters).Build());
            }

            public string Format<T>(string query, Dictionary<string, T> Parameters)
            {
                return Format(query, FormatConfig.Builder().Params(Parameters).Build());
            }

            public string Format(string query, string Indent)
            {
                return Format(query, FormatConfig.Builder().Indent(Indent).Build());
            }

            public string Format(string query)
            {
                return Format(query, FormatConfig.Builder().Build());
            }

            public Formatter Extend(Func<DialectConfig, DialectConfig> oper)
            {
                AbstractFormatter func(FormatConfig config)
                {
                    AbstractFormatter abstractFormatter = new AbstractFormatter(config)
                    {
                        DoDialectConfigFunc = () => oper.Invoke(underlying.Invoke(config).DoDialectConfig())
                    };
                    return abstractFormatter;
                }

                return new Formatter(func);
            }
        }
    }
}
