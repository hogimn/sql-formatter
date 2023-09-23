using Microsoft.AspNetCore.Mvc;

namespace SQL.Formatter.Demo.Controllers
{
    public class SqlFormatterController : Controller
    {
        [HttpPost("/format_sql")]
        public async Task<IActionResult> FormatSql()
        {
            using StreamReader reader = new(Request.Body);
            string requestBody = await reader.ReadToEndAsync();
            string formattedSql = FormatSql(requestBody);
            return Content(formattedSql, "text/plain");
        }

        private static string FormatSql(string sql)
        {
            SqlFormatter.Formatter formatter = SqlFormatter.Standard();
            return formatter.Format(sql);
        }
    }
}
