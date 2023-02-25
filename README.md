# SqlFormatter

This repository contains the C# port of the popular Java SQL formatter <https://github.com/vertical-blank/sql-formatter>,
<br />
which is again the Java port of the popular Typescript SQL formatter https://github.com/sql-formatter-org/sql-formatter

This does not support:

- Stored procedures.
- Changing of the delimiter type to something else than ;.

## Examples

You can easily use `SQL.Formatter.SqlFormatter` :

```c#
SqlFormatter.Format("SELECT * FROM table1")
```

This will output:

```sql
SELECT
  *
FROM
  table1
```

You can also pass `FormatConfig` object built by builder:

```c#
SqlFormatter.Format("SELECT * FROM tbl",
  FormatConfig.Builder()
    .Indent("    ") // Defaults to two spaces
    .Uppercase(true) // Defaults to false (not safe to use when SQL dialect has case-sensitive identifiers)
    .LinesBetweenQueries(2) // Defaults to 1
    .MaxColumnLength(100) // Defaults to 50
    .Params(new List<string>{"a", "b", "c"}) // Dictionary or List. See Placeholders replacement.
    .Build());
);
```

### Dialect

You can pass dialect `SQL.Formatter.Language.Dialect` or `String` to `SqlFormatter.Of` :

```c#
SqlFormatter
    .Of(Dialect.N1ql)  // Recommended
     //.Of("n1ql")      // String can be passed
    .Format("SELECT *");
```

SQL formatter supports the following dialects:

- **sql** - [Standard SQL][]
- **mariadb** - [MariaDB][]
- **mysql** - [MySQL][]
- **postgresql** - [PostgreSQL][]
- **db2** - [IBM DB2][]
- **plsql** - [Oracle PL/SQL][]
- **n1ql** - [Couchbase N1QL][]
- **redshift** - [Amazon Redshift][]
- **spark** - [Spark][]
- **tsql** - [SQL Server Transact-SQL][tsql]

### Extend formatters

Formatters can be extended as below :

```c#
SqlFormatter
    .Of(Dialect.MySql)
    .Extend(cfg => cfg.PlusOperators("=>"))
    .Format("SELECT * FROM table WHERE A => 4")
```

Then it results in:

```sql
SELECT
  *
FROM
  table
WHERE
  A => 4
```

### Placeholders replacement

You can pass `List` or `Dictionary` to `Format` :

```c#
// Named placeholders
Dictionary<string, string> namedParams = new Dictionary<string, string>();
namedParams.Add("foo", "'bar'");
SqlFormatter.Of(Dialect.TSql).Format("SELECT * FROM tbl WHERE foo = @foo", namedParams);

// Indexed placeholders
SqlFormatter.Format("SELECT * FROM tbl WHERE foo = ?", new List<string> {"'bar'"});
```

Both result in:

```sql
SELECT
  *
FROM
  tbl
WHERE
  foo = 'bar'
```

[standard sql]: https://en.wikipedia.org/wiki/SQL:2011
[couchbase n1ql]: http://www.couchbase.com/n1ql
[ibm db2]: https://www.ibm.com/analytics/us/en/technology/db2/
[oracle pl/sql]: http://www.oracle.com/technetwork/database/features/plsql/index.html
[amazon redshift]: https://docs.aws.amazon.com/redshift/latest/dg/cm_chap_SQLCommandRef.html
[spark]: https://spark.apache.org/docs/latest/api/sql/index.html
[postgresql]: https://www.postgresql.org/
[mariadb]: https://mariadb.com/
[mysql]: https://www.mysql.com/
[tsql]: https://docs.microsoft.com/en-us/sql/sql-server/
