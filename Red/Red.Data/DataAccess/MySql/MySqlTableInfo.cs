using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Table {Name}")]
    public class  MySqlTableInfo : ITableInfo
    {
        private bool _alreadyDiscovered = false;
        internal MySqlDatabaseInfo Database { get; set; }

        public string Name { get; internal set; }
        private List<IColumnInfo> _primaryKey { get; } = new List<IColumnInfo>();
        private List<IColumnInfo> _columns { get; } = new List<IColumnInfo>();

        public IEnumerable<IColumnInfo> PrimaryKey => _primaryKey;
        public IEnumerable<IColumnInfo> Columns => _columns;

        public void Discover(IDbConnection connection)
        {
            if (_alreadyDiscovered) return;
            _alreadyDiscovered = true;

            foreach (var columnInfo in connection
                .CreateCommand(
                    @"  SELECT
                            COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE
                            TABLE_SCHEMA=DATABASE() 
                        AND TABLE_NAME=?", 
                    Name)
                .ExecuteEnumerable((r) => 
                    new MySqlColumnInfo()
                    {
                        Table = this,
                        Name = r.GetString(0),
                        DataType = DiscoverDbType(r.GetString(1)),
                        IsNullable = r.GetString(2) == "YES"
                    }))
            {
                _columns.Add(columnInfo);
            }

        }

        private DbType DiscoverDbType(string dataTypeName)
        {
            return Database.Dialect.GetDbTypeFromString(dataTypeName);
        }

        public virtual FetchRequest CreateFetchRequest()
        {
            return new FetchRequest()
            {
                Table = this,
                ColumnsToFetch = Columns.ToArray()
            };
        }


        public MySqlFetchRequest Search(string[] searchTexts, IEnumerable<IColumnInfo> columnsToSearch)
        {
            var request = Database.CreateFetchRequest();
            request.Table = this;

            foreach (var text in searchTexts)
            {
                foreach (var column in columnsToSearch)
                {
                    request.AddPredicate(Database.Dialect.Contains(column, text));
                }
            }

            return request;
        }
        public MySqlFetchRequest Search(string[] searchTexts)
        {
            var searchableColumns = Columns.Where((c) => c.IsSomeString()).ToArray();
            return Search(searchTexts, searchableColumns);
        }
    }
}