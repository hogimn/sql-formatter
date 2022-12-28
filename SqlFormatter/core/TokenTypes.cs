using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core
{
    public enum TokenTypes
    {
        WORD,
        STRING,
        RESERVED,
        RESERVED_TOP_LEVEL,
        RESERVED_TOP_LEVEL_NO_INDENT,
        RESERVED_NEWLINE,
        OPERATOR,
        OPEN_PAREN,
        CLOSE_PAREN,
        LINE_COMMENT,
        BLOCK_COMMENT,
        NUMBER,
        PLACEHOLDER,
    }
}
