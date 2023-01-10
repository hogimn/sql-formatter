using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core
{
    /** Configurations for formatting. */
    public class FormatConfig
    {
        public static readonly string DEFAULT_INDENT = "  ";
        public static readonly int DEFAULT_COLUMN_MAX_LENGTH = 50;

        public readonly string indent;
        public readonly int maxColumnLength;
        public readonly Params parameters;
        public readonly bool uppercase;
        public readonly int linesBetweenQueries;

        public FormatConfig(
            string indent,
            int maxColumnLength,
            Params parameters,
            bool uppercase,
            int linesBetweenQueries)
        {
            this.indent = indent;
            this.maxColumnLength = maxColumnLength;
            this.parameters = parameters == null ? Params.EMPTY : parameters;
            this.uppercase = uppercase;
            this.linesBetweenQueries = linesBetweenQueries;
        }

        /**
         * Returns a new empty Builder.
         *
         * @return A new empty Builder
         */
        public static FormatConfigBuilder Builder()
        {
            return new FormatConfigBuilder();
        }


        /** FormatConfigBuilder */
        public class FormatConfigBuilder
        {
            private string indent = DEFAULT_INDENT;
            private int maxColumnLength = DEFAULT_COLUMN_MAX_LENGTH;
            private Params parameters;
            private bool uppercase;
            private int linesBetweenQueries;

            public FormatConfigBuilder() { }

            /**
             * @param indent Characters used for indentation, default is " " (2 spaces)
             * @return This
             */
            public FormatConfigBuilder Indent(string indent)
            {
                this.indent = indent;
                return this;
            }

            /**
             * @param maxColumnLength Maximum length to treat inline block as one line
             * @return This
             */
            public FormatConfigBuilder MaxColumnLength(int maxColumnLength)
            {
                this.maxColumnLength = maxColumnLength;
                return this;
            }

            /**
             * @param params Collection of params for placeholder replacement
             * @return This
             */
            public FormatConfigBuilder Params(Params parameters)
            {
                this.parameters = parameters;
                return this;
            }

            /**
             * @param params Collection of params for placeholder replacement
             * @return This
            s */
            public FormatConfigBuilder Params<T>(Dictionary<string, T> parameters)
            {
                return Params(core.Params.Of(parameters));
            }

            /**
             * @param params Collection of params for placeholder replacement
             * @return This
            s */
            public FormatConfigBuilder Params<T>(List<T> parameters)
            {
                return Params(core.Params.Of(parameters));
            }

            /**
             * @param uppercase Converts keywords to uppercase
             * @return This
             */
            public FormatConfigBuilder Uppercase(bool uppercase)
            { 
                this.uppercase = uppercase;
                return this;
            }

            /**
             * @param linesBetweenQueries How many line breaks between queries
             * @return This
             */
            public FormatConfigBuilder LinesBetweenQueries(int linesBetweenQueries)
            {
                this.linesBetweenQueries = linesBetweenQueries;
                return this;
            }

            /**
             * Returns an instance of FormatConfig created from the fields set on this builder.
             *
             * @return FormatConfig
             */
            public FormatConfig Build()
            {
                return new FormatConfig(
                    indent, maxColumnLength, parameters, uppercase, linesBetweenQueries);
            }
        }
    }
}
