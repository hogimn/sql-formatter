using SQL.Formatter.Core;
using SQL.Formatter.Language;
using Xunit;

namespace SQL.Formatter.Test
{
    public class FormatConfigTest
    {
        private static readonly SqlFormatter.Formatter s_formatter = SqlFormatter.Of(Dialect.StandardSql);

        [Fact]
        public void Upper_FormatsKeywordsInUppercase()
        {
            var cfg = FormatConfig.Builder().Case(CaseTypes.UPPER).Build();
            var result = s_formatter.Format("select id from users where id = 1", cfg);

            Assert.Equal(
                "SELECT\n"
                + "  id\n"
                + "FROM\n"
                + "  users\n"
                + "WHERE\n"
                + "  id = 1",
                result);
        }

        [Fact]
        public void Uppercase_FormatsKeywordsInUppercase()
        {
#pragma warning disable CS0618
            var cfg = FormatConfig.Builder().Uppercase(true).Build();
#pragma warning restore CS0618
            var result = s_formatter.Format("select id from users where id = 1", cfg);

            Assert.Equal(
                "SELECT\n"
                + "  id\n"
                + "FROM\n"
                + "  users\n"
                + "WHERE\n"
                + "  id = 1",
                result);
        }

        [Fact]
        public void Uppercase_False_PreservesKeywordCase()
        {
#pragma warning disable CS0618
            var cfg = FormatConfig.Builder().Uppercase(false).Build();
#pragma warning restore CS0618
            var result = s_formatter.Format("select id FROM users", cfg);

            Assert.Equal(
                "select\n"
                + "  id\n"
                + "FROM\n"
                + "  users",
                result);
        }

        [Fact]
        public void Lower_FormatsKeywordsInLowercase()
        {
            var cfg = FormatConfig.Builder().Case(CaseTypes.LOWER).Build();
            var result = s_formatter.Format("SELECT id FROM Users WHERE id = 1", cfg);

            Assert.Equal(
                "select\n"
                + "  id\n"
                + "from\n"
                + "  Users\n"
                + "where\n"
                + "  id = 1",
                result);
        }

        [Fact]
        public void Upper_ClearsLower()
        {
            var cfg = FormatConfig.Builder()
                .Case(CaseTypes.LOWER)
                .Case(CaseTypes.UPPER)
                .Build();

            Assert.Equal(CaseTypes.UPPER, cfg.Case);
        }

        [Fact]
        public void Lower_ClearsUpper()
        {
            var cfg = FormatConfig.Builder()
                .Case(CaseTypes.UPPER)
                .Case(CaseTypes.LOWER)
                .Build();

            Assert.Equal(CaseTypes.LOWER, cfg.Case);
        }

        [Fact]
        public void Upper_ClearsLower_FormatsUppercase()
        {
            var cfg = FormatConfig.Builder()
                .Case(CaseTypes.LOWER)
                .Case(CaseTypes.UPPER)
                .Build();

            var result = s_formatter.Format("select id from users", cfg);

            Assert.Equal(
                "SELECT\n"
                + "  id\n"
                + "FROM\n"
                + "  users",
                result);
        }

        [Fact]
        public void Lower_ClearsUpper_FormatsLowercase()
        {
            var cfg = FormatConfig.Builder()
                .Case(CaseTypes.UPPER)
                .Case(CaseTypes.LOWER)
                .Build();

            var result = s_formatter.Format("SELECT id FROM users", cfg);

            Assert.Equal(
                "select\n"
                + "  id\n"
                + "from\n"
                + "  users",
                result);
        }

        [Fact]
        public void NeitherUppercaseNorLowercase_PreservesKeywordCase()
        {
            var cfg = FormatConfig.Builder().Build();
            var result = s_formatter.Format("SeLeCt id FrOm users", cfg);

            Assert.Equal(
                "SeLeCt\n"
                + "  id\n"
                + "FrOm\n"
                + "  users",
                result);
        }

        [Fact]
        public void None_PreservesKeywordCase()
        {
            var cfg = FormatConfig.Builder().Case(CaseTypes.NONE).Build();
            var result = s_formatter.Format("SeLeCt id FrOm users", cfg);

            Assert.Equal(
                "SeLeCt\n"
                + "  id\n"
                + "FrOm\n"
                + "  users",
                result);
        }

        [Theory]
        [InlineData(CaseTypes.NONE, false)]
        [InlineData(CaseTypes.UPPER, true)]
        [InlineData(CaseTypes.LOWER, false)]
        public void Case_SetsUppercaseCorrectly(CaseTypes caseType, bool expectedUppercase)
        {
            var cfg = FormatConfig.Builder().Case(caseType).Build();
#pragma warning disable CS0618
            Assert.Equal(expectedUppercase, cfg.Uppercase);
#pragma warning restore CS0618
        }
    }
}
