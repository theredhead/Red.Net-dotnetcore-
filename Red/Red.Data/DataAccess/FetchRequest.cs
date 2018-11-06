using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Red.Core;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess
{
    public class FetchRequest : ILoadable<XmlElement>, ISavable<XmlElement>
    {
        public IDatabaseInfo Database { get; private set; }

        /// <summary>
        /// List of the properties to fetch, null means all and is the default.
        /// </summary>
        public IColumnInfo[] ColumnsToFetch { get; set; } = null;

        public ITableInfo Table { get; set; }

        private readonly List<FetchPredicate> _predicates = new List<FetchPredicate>();

        public FetchRequest(IDatabaseInfo database)
        {
            Database = database;
        }

        public void AddPredicate(FetchPredicate predicate)
        {
            _predicates.Add(predicate);
        }

        public void RemovePredicate(FetchPredicate predicate)
        {
            _predicates.Remove(predicate);
        }

        public virtual IDbCommand GetCommand(IDbConnection connection)
        {
            Sanity.Enforce<NullReferenceException>(Table != null, nameof(Table));

            ISqlDialect dialect = Database.Dialect;

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
            if (columns.Length == 0)
                columns = "*";

            var commandText = $"SELECT {columns} FROM {table} {where}";

            return connection.CreateCommand(commandText, arguments.ToArray());
        }

        public void Load(XmlElement source)
        {
            Sanity.Enforce<ArgumentException>(
                source.LocalName == nameof(FetchRequest),
                $"Element must be named {nameof(FetchRequest)}");

            _predicates.Clear();
            foreach (XmlElement element in source.SelectNodes($"Predicates/{nameof(FetchPredicate)}"))
            {
                var predicate = new FetchPredicate();
                predicate.Load(element);
                _predicates.Add(predicate);
            }
        }

        public void Save(XmlElement target)
        {
            var document = target.OwnerDocument;
            var element = document.CreateElement(nameof(FetchRequest));

            var predicates = document.CreateElement("Predicates");
            foreach (var predicate in _predicates)
            {
                predicate.Save(predicates);
            }

            target.AppendChild(element);
        }

        public FetchPredicate CreatePredicate()
        {
            return new FetchPredicate();
        }

        public FetchPredicate CreatePredicate(string text, params object[] arguments)
        {
            return new FetchPredicate(text, arguments);
        }

        public PredicateBuilder Where()
        {
            return new PredicateBuilder(this);
        }

        public DataTable FetchDataTable()
        {
            using (var command = GetCommand(Database.CreateConnection()))
            {
                using (var reader = command.ExecuteReader())
                {
                    var table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }
    }

    public class PredicateBuilder
    {
        public FetchRequest Request { get; private set; }

        public PredicateBuilder(FetchRequest request)
        {
            Request = request;
        }

        private string QuotedName(string name)
        {
            return Request.Database.Dialect.QuoteName(name);
        }

        public PredicateBuilder Between<T>(IColumnInfo column, T minimumValue, T maximumValue)
            => Between(column.Name, minimumValue, maximumValue);
        
        public PredicateBuilder Between<T>(string column, T minimumValue, T maximumValue)
        {
            Request.AddPredicate(
                Request.CreatePredicate($"{QuotedName(column)} BETWEEN ? AND ?", minimumValue,
                    maximumValue));
            return this;
        }

        public PredicateBuilder StartsWith(IColumnInfo column, string needle)
            => StartsWith(column.Name, needle); 
        
        public PredicateBuilder StartsWith(string column, string needle)
        {
            Request.AddPredicate(
                Request.CreatePredicate($"LEFT({QuotedName(column)}, {needle.Length}) = ?", needle));
            return this;
        }

        public PredicateBuilder EndsWith(IColumnInfo column, string needle)
            => EndsWith(column.Name, needle);
        
        public PredicateBuilder EndsWith(string column, string needle)
        {
            Request.AddPredicate(
                Request.CreatePredicate($"RIGHT({QuotedName(column)}, {needle.Length}) = ?", needle));
            return this;
        }

        public PredicateBuilder Contains(IColumnInfo column, string needle)
            => Contains(column.Name, needle);
        
        public PredicateBuilder Contains(string column, string needle)
        {
            Request.AddPredicate(
                Request.CreatePredicate($"INSTR({QuotedName(column)}, ?) > 0", needle));
            return this;
        }

        public PredicateBuilder Equals(IColumnInfo column, object needle)
            => Equals(column.Name, needle);

        public PredicateBuilder Equals(string column, object needle)
        {
            Request.AddPredicate(
                Request.CreatePredicate($"{QuotedName(column)} = ?", needle));
            return this;
        }
    }
}