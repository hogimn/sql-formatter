using System.Collections.Generic;
using System.Linq;

namespace SQL.Formatter.Core
{
    public class Indentation
    {
        private enum IndentTypes
        {
            INDENT_TYPE_TOP_LEVEL,
            INDENT_TYPE_BLOCK_LEVEL
        }

        private readonly string _indent;
        private readonly Stack<IndentTypes> _indentTypes;

        public Indentation(string indent)
        {
            _indent = indent;
            _indentTypes = new Stack<IndentTypes>();
        }

        public string GetIndent() =>
            string.Concat(Enumerable.Range(0, _indentTypes.Count)
                .Select(_ => _indent));

        public void IncreaseTopLevel()
        {
            _indentTypes.Push(IndentTypes.INDENT_TYPE_TOP_LEVEL);
        }

        public void IncreaseBlockLevel()
        {
            _indentTypes.Push(IndentTypes.INDENT_TYPE_BLOCK_LEVEL);
        }

        public void DecreaseTopLevel()
        {
            if (_indentTypes.Count != 0 &&
                _indentTypes.Peek() == IndentTypes.INDENT_TYPE_TOP_LEVEL)
            {
                _indentTypes.Pop();
            }
        }

        public void DecreaseBlockLevel()
        {
            while (_indentTypes.Count > 0)
            {
                var type = _indentTypes.Pop();
                if (type != IndentTypes.INDENT_TYPE_TOP_LEVEL)
                {
                    break;
                }
            }
        }

        public void ResetIndentation()
        {
            _indentTypes.Clear();
        }
    }
}
