using SQL.Formatter.Core;

namespace SQL.Formatter.Language
{
    public interface IDialectConfigurator
    {
        DialectConfig DoDialectConfig();
    }
}
