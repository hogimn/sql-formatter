using VerticalBlank.SqlFormatter.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.languages
{
    public interface DialectConfigurator
    {
        DialectConfig DoDialectConfig();
    }
}
