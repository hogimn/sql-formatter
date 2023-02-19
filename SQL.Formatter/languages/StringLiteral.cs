using System.Collections.Generic;
using System.Linq;

namespace SQL.Formatter.languages
{
    public class StringLiteral
    {
        public static readonly string BACK_QUOTE = "``";
        public static readonly string DOUBLE_QUOTE = "\"\"";
        public static readonly string U_DOUBLE_QUOTE = "U&\"\"";
        public static readonly string U_SINGLE_QUOTE = "U&''";
        public static readonly string E_SINGLE_QUOTE = "E''";
        public static readonly string N_SINGLE_QUOTE = "N''";
        public static readonly string Q_SINGLE_QUOTE = "Q''";
        public static readonly string SINGLE_QUOTE = "''";
        public static readonly string BRACE = "{}";
        public static readonly string DOLLAR = "$$";
        public static readonly string BRACKET = "[]";

        private static readonly Dictionary<string, string> literals;

        static StringLiteral()
        {
            literals = Preset.Presets.ToList()
                .ToDictionary(preset => preset.GetKey(), preset => preset.GetRegex());
        }

        public static string Get(string key) => literals[key];

        private class Preset
        {
            /** `` */
            public static readonly Preset BACK_QUOTE = new Preset(
                StringLiteral.BACK_QUOTE,
                "((`[^`]*($|`))+)"
                );
            /** "" */
            public static readonly Preset DOUBLE_QUOTE = new Preset(
                StringLiteral.DOUBLE_QUOTE,
                "((\"[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*(\"|$))+)"
                // "((^\"((?:\"\"|[^\"])*)\")+)")
                );
            /** [] */
            public static readonly Preset BRACKET = new Preset(
                StringLiteral.BRACKET,
                "((\\[[^\\]]*($|\\]))(\\][^\\]]*($|\\]))*)"
                );
            /** {} */
            public static readonly Preset BRACE = new Preset(
                StringLiteral.BRACE,
                "((\\{[^\\}]*($|\\}))+)"
                );
            /** '' */
            public static readonly Preset SINGLE_QUOTE = new Preset(
                StringLiteral.SINGLE_QUOTE,
                "(('[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                // "((^'((?:''|[^'])*)')+)")
                );
            /** N'' */
            public static readonly Preset N_SINGLE_QUOTE = new Preset(
                StringLiteral.N_SINGLE_QUOTE,
                "((N'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );
            /** q'' */
            public static readonly Preset Q_SINGLE_QUOTE = new Preset(
                StringLiteral.Q_SINGLE_QUOTE,
                "(?i)" +
                string.Join(
                    "|",
                    "((n?q'\\{(?:(?!\\}'|\\\\).)*\\}')+)",
                    "((n?q'\\[(?:(?!\\]'|\\\\).)*\\]')+)",
                    "((n?q'<(?:(?!>'|\\\\).)*>')+)",
                    "((n?q'\\((?:(?!\\)'|\\\\).)*\\)')+)"));
            // single_quote("((^'((?:''|[^'])*)')+)")
            public static readonly Preset E_SINGLE_QUOTE = new Preset(
                StringLiteral.E_SINGLE_QUOTE,
                "((E'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );
            /** U&amp;'' */
            public static readonly Preset U_SINGLE_QUOTE = new Preset(
                StringLiteral.U_SINGLE_QUOTE,
                "((U&'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );
            /** U&amp;"" */
            public static readonly Preset U_DOUBLE_QUOTE = new Preset(
                StringLiteral.U_DOUBLE_QUOTE,
                "((U&\"[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*(\"|$))+)"
                );
            /** $$ */
            public static readonly Preset DOLLAR = new Preset(
                StringLiteral.DOLLAR,
                "((?<tag>\\$\\w*\\$)[\\s\\S]*?(?:\\k<tag>|$))"
                );

            public static IEnumerable<Preset> Presets
            {
                get
                {
                    yield return BACK_QUOTE;
                    yield return DOUBLE_QUOTE;
                    yield return BRACKET;
                    yield return BRACE;
                    yield return SINGLE_QUOTE;
                    yield return N_SINGLE_QUOTE;
                    yield return Q_SINGLE_QUOTE;
                    yield return E_SINGLE_QUOTE;
                    yield return U_SINGLE_QUOTE;
                    yield return U_DOUBLE_QUOTE;
                    yield return DOLLAR;

                }
            }

            public readonly string key;
            public readonly string regex;

            public Preset(string key, string regex)
            {
                this.key = key;
                this.regex = regex;
            }

            public string GetKey() => key;

            public string GetRegex() => regex;
        }
    }
}
