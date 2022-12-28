using VerticalBlank.SqlFormatter.core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core
{


    /**
     * Bookkeeper for inline blocks.
     *
     * <p>Inline blocks are parenthized expressions that are shorter than maxColumnLength. These blocks
     * are formatted on a single line, unlike longer parenthized expressions where open-parenthesis
     * causes newline and increase of indentation.
     */
    public class InlineBlock
    {
        private int level;
        private readonly int maxColumnLength;

        public InlineBlock(int maxColumnLength)
        {
            this.maxColumnLength = maxColumnLength;
            level = 0;
        }

        /**
         * Begins inline block when lookahead through upcoming tokens determines that the block would be
         * smaller than INLINE_MAX_LENGTH.
         *
         * @param tokens Array of all tokens
         * @param index Current token position
         */
        public void BeginIfPossible(JSLikeList<Token> tokens, int index)
        {
            if (level == 0 && IsInlineBlock(tokens, index))
            {
                level = 1;
            }
            else if (level > 0)
            {
                level++;
            }
            else
            {
                level = 0;
            }
        }

        /** Finishes current inline block. There might be several nested ones. */
        public void End()
        {
            level--;
        }

        /**
         * True when inside an inline block
         *
         * @return {Boolean}
         */
        public bool IsActive()
        {
            return level > 0;
        }

        // Check if this should be an inline parentheses block
        // Examples are "NOW()", "COUNT(*)", "int(10)", key(`somecolumn`), DECIMAL(7,2)
        private bool IsInlineBlock(JSLikeList<Token> tokens, int index)
        {
            int length = 0;
            int level = 0;

            for (int i = index; i < tokens.Size(); i++) {
                Token token = tokens.Get(i);
                length += token.value.Length;

                // Overran max length
                if (length > maxColumnLength)
                    return false;

                if (token.type == TokenTypes.OPEN_PAREN)
                {
                    level++;
                }
                else if (token.type == TokenTypes.CLOSE_PAREN)
                {
                    level--;
                    if (level == 0)
                        return true;
                }

                if (IsForbiddenToken(token))
                    return false;
            }

            return false;
        }

        // Reserved words that cause newlines, comments and semicolons
        // are not allowed inside inline parentheses block
        private bool IsForbiddenToken(Token token)
        {
            return token.type == TokenTypes.RESERVED_TOP_LEVEL
                || token.type == TokenTypes.RESERVED_NEWLINE
                ||
                //                originally `TokenTypes.LINE_COMMENT` but this symbol is not defined
                //                token.type == TokenTypes.LINE_COMMENT ||
                token.type == TokenTypes.BLOCK_COMMENT
                || token.value.Equals(";");
        }
    }
}
