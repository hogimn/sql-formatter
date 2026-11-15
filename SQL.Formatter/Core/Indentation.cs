using System.Collections.Generic;
using System.Text;

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
        private readonly StringBuilder _indentBuilder;
        private readonly int _indentLength;

        public Indentation(string indent)
        {
            _indent = indent;
            _indentTypes = new Stack<IndentTypes>();
            _indentBuilder = new StringBuilder();
            _indentLength = indent == null ? 0 : indent.Length;
        }

        public string GetIndent() => _indentBuilder.ToString();

        public void AppendTo(StringBuilder builder)
        {
            builder.Append(_indentBuilder);
        }

        public void IncreaseTopLevel()
        {
            _indentTypes.Push(IndentTypes.INDENT_TYPE_TOP_LEVEL);
            _indentBuilder.Append(_indent);
        }

        public void IncreaseBlockLevel()
        {
            _indentTypes.Push(IndentTypes.INDENT_TYPE_BLOCK_LEVEL);
            _indentBuilder.Append(_indent);
        }

        public void DecreaseTopLevel()
        {
            if (_indentTypes.Count != 0 &&
                _indentTypes.Peek() == IndentTypes.INDENT_TYPE_TOP_LEVEL)
            {
                _indentTypes.Pop();
                RemoveOneIndent();
            }
        }

        public void DecreaseBlockLevel()
        {
            while (_indentTypes.Count > 0)
            {
                var type = _indentTypes.Pop();
                RemoveOneIndent();
                if (type != IndentTypes.INDENT_TYPE_TOP_LEVEL)
                {
                    break;
                }
            }
        }

        public void ResetIndentation()
        {
            _indentTypes.Clear();
            _indentBuilder.Clear();
        }

        private void RemoveOneIndent()
        {
            _indentBuilder.Length -= _indentLength;
        }
    }
}
