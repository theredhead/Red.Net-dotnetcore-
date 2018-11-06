using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Table {Name}")]
    public class  MySqlTableInfo : TableInfo
    {
        private bool _alreadyDiscovered = false;
        private List<IColumnInfo> _primaryKey { get; } = new List<IColumnInfo>();
        private List<IColumnInfo> _columns { get; } = new List<IColumnInfo>();

        public override void Discover(IDbConnection connection)
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
            return new FetchRequest(Database)
            {
                Table = this,
                ColumnsToFetch = Columns.ToArray()
            };
        }
    }
}