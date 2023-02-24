using System.Collections.Generic;
using System.Linq;

namespace SQL.Formatter.Core
{
    /**
     * Manages indentation levels.
     *
     * <p>There are two types of indentation levels:
     *
     * <p>- BLOCK_LEVEL : increased by open-parenthesis - TOP_LEVEL : increased by RESERVED_TOPLEVEL
     * words
     */
    public class Indentation
    {
        enum IndentTypes
        {
            INDENT_TYPE_TOP_LEVEL,
            INDENT_TYPE_BLOCK_LEVEL
        }

        private readonly string indent;
        private readonly Stack<IndentTypes> indentTypes;

        /**
         * @param indent Indent value, default is " " (2 spaces)
         */
        public Indentation(string indent)
        {
            this.indent = indent;
            indentTypes = new Stack<IndentTypes>();
        }

        /**
         * Returns current indentation string.
         *
         * @return {String}
         */
        public string GetIndent() =>
            string.Concat(Enumerable.Range(0, indentTypes.Count)
                .Select(_ => indent));

        /** Increases indentation by one top-level indent. */
        public void IncreaseTopLevel()
        {
            indentTypes.Push(IndentTypes.INDENT_TYPE_TOP_LEVEL);
        }

        /** Increases indentation by one block-level indent. */
        public void IncreaseBlockLevel()
        {
            indentTypes.Push(IndentTypes.INDENT_TYPE_BLOCK_LEVEL);
        }

        /**
         * Decreases indentation by one top-level indent. Does nothing when the previous indent is not
         * top-level.
         */
        public void DecreaseTopLevel()
        {
            if (indentTypes.Count != 0 &&
                indentTypes.Peek() == IndentTypes.INDENT_TYPE_TOP_LEVEL)
            {
                indentTypes.Pop();
            }
        }

        /**
         * Decreases indentation by one block-level indent. If there are top-level indents within the
         * block-level indent, throws away these as well.
         */
        public void DecreaseBlockLevel()
        {
            while (indentTypes.Count > 0)
            {
                IndentTypes type = indentTypes.Pop();
                if (type != IndentTypes.INDENT_TYPE_TOP_LEVEL)
                {
                    break;
                }
            }
        }

        public void ResetIndentation()
        {
            indentTypes.Clear();
        }
    }
}
