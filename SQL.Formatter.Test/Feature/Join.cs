using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace SQL.Formatter.Test.Feature
{
    public class Join
    {
        public static void Test(SqlFormatter.Formatter formatter,
            List<string> without = null,
            List<string> additionally = null)
        {
            if (without == null) without = new List<string>();
            if (additionally == null) additionally = new List<string>();

            var unsupportedJoinRegex = without.Count > 0 ? string.Join("|", without) : "^whateve_!%&$";

            bool IsSupportedJoin(string join) => Regex.Matches(join, unsupportedJoinRegex).Count == 0;

            SupportsJoin(formatter, IsSupportedJoin, additionally);
        }

        private static void SupportsJoin(
            SqlFormatter.Formatter formatter,
            Func<string, bool> IsSupportedJoin,
            List<string> additionally)
        {
            new List<string> { "CROSS JOIN", "NATURAL JOIN" }
                .Where(IsSupportedJoin)
                .ToList()
                .ForEach(join =>
                {
                    Assert.Equal(
                        "SELECT\n"
                        + "  *\n"
                        + "FROM\n"
                        + "  tbl1\n"
                        + $"  {join} tbl2",
                        formatter.Format($"SELECT * FROM tbl1 {join} tbl2"));
                });

            new List<string> {
                    "JOIN",
                    "INNER JOIN",
                    "LEFT JOIN",
                    "LEFT OUTER JOIN",
                    "RIGHT JOIN",
                    "RIGHT OUTER JOIN",
                    "FULL JOIN",
                    "FULL OUTER JOIN"
                }.Concat(additionally)
                .Where(IsSupportedJoin)
                .ToList()
                .ForEach(join =>
                {
                    Assert.Equal(
                        "SELECT\n"
                        + "  customer_id.from,\n"
                        + "  COUNT(order_id) AS total\n"
                        + "FROM\n"
                        + "  customers\n"
                        + $"  {join} orders ON customers.customer_id = orders.customer_id;",
                        formatter.Format(
                            "SELECT customer_id.from, COUNT(order_id) AS total FROM customers\n"
                            + $"{join} orders ON customers.customer_id = orders.customer_id;"));
                });
        }
    }
}