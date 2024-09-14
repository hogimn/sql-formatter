using SQL.Formatter.Core.Util;

namespace SQL.Formatter.Core
{
    public class InlineBlock
    {
        private int _level;
        private readonly int _maxColumnLength;

        public InlineBlock(int maxColumnLength)
        {
            _maxColumnLength = maxColumnLength;
            _level = 0;
        }

        public void BeginIfPossible(JSLikeList<Token> tokens, int index)
        {
            if (_level == 0 && IsInlineBlock(tokens, index))
            {
                _level = 1;
            }
            else if (_level > 0)
            {
                _level++;
            }
            else
            {
                _level = 0;
            }
        }

        public void End()
        {
            _level--;
        }

        public bool IsActive()
        {
            return _level > 0;
        }

        private bool IsInlineBlock(JSLikeList<Token> tokens, int index)
        {
            var length = 0;
            var level = 0;

            for (var i = index; i < tokens.Size(); i++)
            {
                var token = tokens.Get(i);
                length += token.Value.Length;

                if (length > _maxColumnLength)
                {
                    return false;
                }

                if (token.Type == TokenTypes.OPEN_PAREN)
                {
                    level++;
                }
                else if (token.Type == TokenTypes.CLOSE_PAREN)
                {
                    level--;
                    if (level == 0)
                    {
                        return true;
                    }
                }

                if (IsForbiddenToken(token))
                {
                    return false;
                }
            }

            return false;
        }

        private static bool IsForbiddenToken(Token token)
        {
            return token.Type == TokenTypes.RESERVED_TOP_LEVEL
                || token.Type == TokenTypes.RESERVED_NEWLINE
                || token.Type == TokenTypes.BLOCK_COMMENT
                || token.Value.Equals(";");
        }
    }
}
