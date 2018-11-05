using System.Data;
using System.Diagnostics;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Column {Name}")]
    public class MySqlColumnInfo : IColumnInfo
    {
        public MySqlTableInfo Table { get; set; }
        public string Name { get; internal set;  }
        public DbType DataType { get; internal set; }
        public bool IsNullable { get; internal set; }
    }
}