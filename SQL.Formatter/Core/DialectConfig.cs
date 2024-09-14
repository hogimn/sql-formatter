using System.Collections.Generic;
using System.Linq;
using SQL.Formatter.Core.Util;

namespace SQL.Formatter.Core
{
    public class DialectConfig
    {
        public readonly List<string> LineCommentTypes;
        public readonly List<string> ReservedTopLevelWords;
        public readonly List<string> ReservedTopLevelWordsNoIndent;
        public readonly List<string> ReservedNewlineWords;
        public readonly List<string> ReservedWords;
        public readonly List<string> SpecialWordChars;
        public readonly List<string> StringTypes;
        public readonly List<string> OpenParens;
        public readonly List<string> CloseParens;
        public readonly List<string> IndexedPlaceholderTypes;
        public readonly List<string> NamedPlaceholderTypes;
        public readonly List<string> Operators;

        private DialectConfig(
            List<string> lineCommentTypes,
            List<string> reservedTopLevelWords,
            List<string> reservedTopLevelWordsNoIndent,
            List<string> reservedNewlineWords,
            List<string> reservedWords,
            List<string> specialWordChars,
            List<string> stringTypes,
            List<string> openParens,
            List<string> closeParens,
            List<string> indexedPlaceholderTypes,
            List<string> namedPlaceholderTypes,
            List<string> operators)
        {
            LineCommentTypes = Utils.NullToEmpty(lineCommentTypes);
            ReservedTopLevelWords = Utils.NullToEmpty(reservedTopLevelWords);
            ReservedTopLevelWordsNoIndent = Utils.NullToEmpty(reservedTopLevelWordsNoIndent);
            ReservedNewlineWords = Utils.NullToEmpty(reservedNewlineWords);
            ReservedWords = Utils.NullToEmpty(reservedWords);
            SpecialWordChars = Utils.NullToEmpty(specialWordChars);
            StringTypes = Utils.NullToEmpty(stringTypes);
            OpenParens = Utils.NullToEmpty(openParens);
            CloseParens = Utils.NullToEmpty(closeParens);
            IndexedPlaceholderTypes = Utils.NullToEmpty(indexedPlaceholderTypes);
            NamedPlaceholderTypes = Utils.NullToEmpty(namedPlaceholderTypes);
            Operators = Utils.NullToEmpty(operators);
        }
        public DialectConfig WithLineCommentTypes(List<string> lineCommentTypes)
        {
            return ToBuilder()
                .LineCommentTypes(lineCommentTypes)
                .Build();
        }

        public DialectConfig PlusLineCommentTypes(params string[] lineCommentTypes)
        {
            return PlusLineCommentTypes(lineCommentTypes.ToList());
        }

        public DialectConfig PlusLineCommentTypes(List<string> lineCommentTypes)
        {
            return ToBuilder()
                .LineCommentTypes(Utils.Concat(LineCommentTypes, lineCommentTypes))
                .Build();
        }

        public DialectConfig WithReservedTopLevelWords(List<string> reservedTopLevelWords)
        {
            return ToBuilder()
                .ReservedTopLevelWords(reservedTopLevelWords)
                .Build();
        }

        public DialectConfig PlusReservedTopLevelWords(params string[] reservedTopLevelWords)
        {
            return PlusReservedTopLevelWords(reservedTopLevelWords.ToList());
        }

        public DialectConfig PlusReservedTopLevelWords(List<string> reservedTopLevelWords)
        {
            return ToBuilder()
                .ReservedTopLevelWords(Utils.Concat(ReservedTopLevelWords, reservedTopLevelWords))
                .Build();
        }

        public DialectConfig WithReservedNewlineWords(List<string> reservedNewLineWords)
        {
            return ToBuilder()
                .ReservedNewlineWords(reservedNewLineWords)
                .Build();
        }

        public DialectConfig PlusReservedNewlineWords(params string[] reservedNewLineWords)
        {
            return PlusReservedNewlineWords(reservedNewLineWords.ToList());
        }

        public DialectConfig PlusReservedNewlineWords(List<string> reservedNewlineWords)
        {
            return ToBuilder()
                .ReservedNewlineWords(Utils.Concat(ReservedNewlineWords, reservedNewlineWords))
                .Build();
        }

        public DialectConfig WithReservedTopLevelWordsNoIndent(List<string> reservedTopLevelWordsNoIndent)
        {
            return ToBuilder()
                .ReservedTopLevelWordsNoIndent(reservedTopLevelWordsNoIndent)
                .Build();
        }

        public DialectConfig PlusReservedTopLevelWordsNoIndent(params string[] reservedTopLevelWordsNoIndent)
        {
            return PlusReservedTopLevelWordsNoIndent(reservedTopLevelWordsNoIndent.ToList());
        }

        public DialectConfig PlusReservedTopLevelWordsNoIndent(List<string> reservedTopLevelWordsNoIndent)
        {
            return ToBuilder()
                .ReservedTopLevelWordsNoIndent(Utils.Concat(ReservedTopLevelWordsNoIndent, reservedTopLevelWordsNoIndent))
                .Build();
        }

        public DialectConfig WithReservedWords(List<string> reservedWords)
        {
            return ToBuilder()
                .ReservedWords(reservedWords)
                .Build();
        }

        public DialectConfig PlusReservedWords(params string[] reservedWords)
        {
            return PlusReservedWords(reservedWords.ToList());
        }

        public DialectConfig PlusReservedWords(List<string> reservedWords)
        {
            return ToBuilder()
                .ReservedWords(Utils.Concat(ReservedWords, reservedWords))
                .Build();
        }

        public DialectConfig WithSpecialWordChars(List<string> specialWordChars)
        {
            return ToBuilder()
                .SpecialWordChars(specialWordChars)
                .Build();
        }

        public DialectConfig PlusSpecialWordChars(params string[] specialWordChars)
        {
            return PlusSpecialWordChars(specialWordChars.ToList());
        }

        public DialectConfig PlusSpecialWordChars(List<string> specialWordChars)
        {
            return ToBuilder()
                .SpecialWordChars(Utils.Concat(SpecialWordChars, specialWordChars))
                .Build();
        }

        public DialectConfig WithStringTypes(List<string> stringTypes)
        {
            return ToBuilder()
                .StringTypes(stringTypes)
                .Build();
        }

        public DialectConfig PlusStringTypes(params string[] stringTypes)
        {
            return PlusStringTypes(stringTypes.ToList());
        }

        public DialectConfig PlusStringTypes(List<string> stringTypes)
        {
            return ToBuilder()
                .StringTypes(Utils.Concat(StringTypes, stringTypes))
                .Build();
        }

        public DialectConfig WithOpenParens(List<string> openParens)
        {
            return ToBuilder()
                .OpenParens(openParens)
                .Build();
        }

        public DialectConfig PlusOpenParens(params string[] openParens)
        {
            return PlusOpenParens(openParens.ToList());
        }

        public DialectConfig PlusOpenParens(List<string> openParens)
        {
            return ToBuilder()
                .OpenParens(Utils.Concat(OpenParens, openParens))
                .Build();
        }

        public DialectConfig WithCloseParens(List<string> closeParens)
        {
            return ToBuilder()
                .CloseParens(closeParens)
                .Build();
        }

        public DialectConfig PlusCloseParens(params string[] closeParens)
        {
            return PlusCloseParens(closeParens.ToList());
        }

        public DialectConfig PlusCloseParens(List<string> closeParens)
        {
            return ToBuilder()
                .CloseParens(Utils.Concat(CloseParens, closeParens))
                .Build();
        }

        public DialectConfig WithIndexedPlaceholderTypes(List<string> indexedPlaceholderTypes)
        {
            return ToBuilder()
                .IndexedPlaceholderTypes(indexedPlaceholderTypes)
                .Build();
        }

        public DialectConfig PlusIndexedPlaceholderTypes(params string[] indexedPlaceholderTypes)
        {
            return PlusIndexedPlaceholderTypes(indexedPlaceholderTypes.ToList());
        }

        public DialectConfig PlusIndexedPlaceholderTypes(List<string> indexedPlaceholderTypes)
        {
            return ToBuilder()
                .IndexedPlaceholderTypes(Utils.Concat(IndexedPlaceholderTypes, indexedPlaceholderTypes))
                .Build();
        }

        public DialectConfig WithNamedPlaceholderTypes(List<string> namedPlaceholderTypes)
        {
            return ToBuilder()
                .NamedPlaceholderTypes(namedPlaceholderTypes)
                .Build();
        }

        public DialectConfig PlusNamedPlaceholderTypes(params string[] namedPlaceholderTypes)
        {
            return PlusNamedPlaceholderTypes(namedPlaceholderTypes.ToList());
        }

        public DialectConfig PlusNamedPlaceholderTypes(List<string> namedPlaceholderTypes)
        {
            return ToBuilder()
                .NamedPlaceholderTypes(Utils.Concat(NamedPlaceholderTypes, namedPlaceholderTypes))
                .Build();
        }

        public DialectConfig WithOperators(List<string> operators)
        {
            return ToBuilder()
                .Operators(operators)
                .Build();
        }

        public DialectConfig PlusOperators(params string[] operators)
        {
            return PlusOperators(operators.ToList());
        }

        public DialectConfig PlusOperators(List<string> operators)
        {
            return ToBuilder()
                .Operators(Utils.Concat(Operators, operators))
                .Build();
        }

        public DialectConfigBuilder ToBuilder()
        {
            return Builder()
                .LineCommentTypes(LineCommentTypes)
                .ReservedTopLevelWords(ReservedTopLevelWords)
                .ReservedTopLevelWordsNoIndent(ReservedTopLevelWordsNoIndent)
                .ReservedNewlineWords(ReservedNewlineWords)
                .ReservedWords(ReservedWords)
                .SpecialWordChars(SpecialWordChars)
                .StringTypes(StringTypes)
                .OpenParens(OpenParens)
                .CloseParens(CloseParens)
                .IndexedPlaceholderTypes(IndexedPlaceholderTypes)
                .NamedPlaceholderTypes(NamedPlaceholderTypes)
                .Operators(Operators);
        }

        public static DialectConfigBuilder Builder()
        {
            return new DialectConfigBuilder();
        }

        public class DialectConfigBuilder
        {
            private List<string> lineCommentTypes;
            private List<string> reservedTopLevelWords;
            private List<string> reservedTopLevelWordsNoIndent;
            private List<string> reservedNewlineWords;
            private List<string> reservedWords;
            private List<string> specialWordChars;
            private List<string> stringTypes;
            private List<string> openParens;
            private List<string> closeParens;
            private List<string> indexedPlaceholderTypes;
            private List<string> namedPlaceholderTypes;
            private List<string> operators;

            public DialectConfigBuilder LineCommentTypes(List<string> lineCommentTypes)
            {
                this.lineCommentTypes = lineCommentTypes;
                return this;
            }

            public DialectConfigBuilder ReservedTopLevelWords(List<string> reservedTopLevelWords)
            {
                this.reservedTopLevelWords = reservedTopLevelWords;
                return this;
            }

            public DialectConfigBuilder ReservedTopLevelWordsNoIndent(List<string> reservedTopLevelWordsNoIndent)
            {
                this.reservedTopLevelWordsNoIndent = reservedTopLevelWordsNoIndent;
                return this;
            }

            public DialectConfigBuilder ReservedNewlineWords(List<string> reservedNewlineWords)
            {
                this.reservedNewlineWords = reservedNewlineWords;
                return this;
            }

            public DialectConfigBuilder ReservedWords(List<string> reservedWords)
            {
                this.reservedWords = reservedWords;
                return this;
            }

            public DialectConfigBuilder SpecialWordChars(List<string> specialWordChars)
            {
                this.specialWordChars = specialWordChars;
                return this;
            }

            public DialectConfigBuilder StringTypes(List<string> stringTypes)
            {
                this.stringTypes = stringTypes;
                return this;
            }

            public DialectConfigBuilder OpenParens(List<string> openParens)
            {
                this.openParens = openParens;
                return this;
            }

            public DialectConfigBuilder CloseParens(List<string> closeParens)
            {
                this.closeParens = closeParens;
                return this;
            }

            public DialectConfigBuilder IndexedPlaceholderTypes(List<string> indexedPlaceholderTypes)
            {
                this.indexedPlaceholderTypes = indexedPlaceholderTypes;
                return this;
            }

            public DialectConfigBuilder NamedPlaceholderTypes(List<string> namedPlaceholderTypes)
            {
                this.namedPlaceholderTypes = namedPlaceholderTypes;
                return this;
            }

            public DialectConfigBuilder Operators(List<string> operators)
            {
                this.operators = operators;
                return this;
            }

            public DialectConfig Build()
            {
                return new DialectConfig(
                    lineCommentTypes,
                    reservedTopLevelWords,
                    reservedTopLevelWordsNoIndent,
                    reservedNewlineWords,
                    reservedWords,
                    specialWordChars,
                    stringTypes,
                    openParens,
                    closeParens,
                    indexedPlaceholderTypes,
                    namedPlaceholderTypes,
                    operators);
            }
        }
    }
}
