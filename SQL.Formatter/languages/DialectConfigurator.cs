using SQL.Formatter.core;

namespace SQL.Formatter.languages
{
    public interface IDialectConfigurator
    {
        DialectConfig DoDialectConfig();
    }
}
