﻿using System;
using System.Collections.Generic;
using System.Linq;
using SQL.Formatter.Core;

namespace SQL.Formatter.Language
{
    public class Dialect
    {
        public static readonly Dialect Db2 = new Dialect(cfg => new Db2Formatter(cfg), "Db2");
        public static readonly Dialect MariaDb = new Dialect(cfg => new MariaDbFormatter(cfg), "MariaDb");
        public static readonly Dialect MySql = new Dialect(cfg => new MySqlFormatter(cfg), "MySql");
        public static readonly Dialect N1ql = new Dialect(cfg => new N1qlFormatter(cfg), "N1ql");
        public static readonly Dialect PlSql = new Dialect(cfg => new PlSqlFormatter(cfg), "PlSql", "pl/sql");
        public static readonly Dialect PostgreSql = new Dialect(cfg => new PostgreSqlFormatter(cfg), "PostgreSql");
        public static readonly Dialect Redshift = new Dialect(cfg => new RedshiftFormatter(cfg), "Redshift");
        public static readonly Dialect SparkSql = new Dialect(cfg => new SparkSqlFormatter(cfg), "SparkSql", "spark");
        public static readonly Dialect StandardSql = new Dialect(cfg => new StandardSqlFormatter(cfg), "StandardSql", "sql");
        public static readonly Dialect TSql = new Dialect(cfg => new TSqlFormatter(cfg), "TSql");

        public static IEnumerable<Dialect> Values
        {
            get
            {
                yield return Db2;
                yield return MariaDb;
                yield return MySql;
                yield return N1ql;
                yield return PlSql;
                yield return PostgreSql;
                yield return Redshift;
                yield return SparkSql;
                yield return StandardSql;
                yield return TSql;
            }
        }

        public readonly string Name;
        public readonly Func<FormatConfig, AbstractFormatter> Func;
        public readonly List<string> Aliases;

        private Dialect(Func<FormatConfig, AbstractFormatter> func, string name, params string[] aliases)
        {
            Func = func;
            Name = name;
            Aliases = new List<string>(aliases);
        }

        private bool Matches(string name)
        {
            return Name.ToLower().Equals(name.ToLower())
                || Aliases.Select(s => s.ToLower()).Intersect(new string[] { name.ToLower() }).Any();
        }

        public static Dialect NameOf(string name)
        {
            var dialects = Values.Where(d => d.Matches(name));

            if (!dialects.Any())
            {
                return null;
            }

            return dialects.First();
        }
    }
}
