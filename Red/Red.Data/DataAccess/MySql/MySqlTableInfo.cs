using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Table {Name}")]
    public class  MySqlTableInfo : TableInfo
    {
        private bool _alreadyDiscovered;
        private List<IColumnInfo> _primaryKey { get; } = new List<IColumnInfo>();

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
                _Columns.Add(columnInfo.Name, columnInfo);
            }

        }

        private DbType DiscoverDbType(string dataTypeName)
        {
            return Database.Dialect.GetDbTypeFromString(dataTypeName);
        }
    }
}