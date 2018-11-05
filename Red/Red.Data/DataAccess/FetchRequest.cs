using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Red.Core;

namespace Red.Data.DataAccess
{
    public class FetchRequest
    {
        /// <summary>
        /// List of the properties to fetch, null means all and is the default.
        /// </summary>
        public IColumnInfo[] ColumnsToFetch { get; set; } = null;
        public ITableInfo Table { get; set; }

        private readonly List<FetchPredicate> _predicates = new List<FetchPredicate>();

        public void AddPredicate(FetchPredicate predicate)
        {
            _predicates.Add(predicate);
        }
        public void RemovePredicate(FetchPredicate predicate)
        {
            _predicates.Remove(predicate);
        }

        public virtual IDbCommand GetCommand(IDbConnection connection, ISqlDialect dialect)
        {
            Sanity.Enforce<NullReferenceException>(Table != null, nameof(Table));

            var arguments = new List<object>();
            var snippets = new List<string>();

            foreach (var predicate in _predicates)
            {
                snippets.Add($"({predicate.Text})");
                arguments.AddRange(predicate.Arguments);
            }

            var where = snippets.Count > 0
                ? "WHERE " + string.Join(" AND ", snippets)
                : "";

            var table = dialect.QuoteName(Table.Name);
            var columns = string.Join(", ", (
                from column in Table.Columns
                where ColumnsToFetch?.Contains(column) ?? true
                select dialect.QuoteName(column.Name)
            ));
                
            var commandText = $"SELECT {columns} FROM {table} {where}";

            return connection.CreateCommand(commandText, arguments.ToArray());
        }
    }
}