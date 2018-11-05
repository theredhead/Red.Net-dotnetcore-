using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Database {Name}")]
    public class MySqlDatabaseInfo : IDatabaseInfo
    {
        private bool _alreadyDiscovered = false;
        public ISqlDialect Dialect { get; internal set; }
        public string Name { get; internal set; }
        public IEnumerable<ITableInfo> Tables { get; internal set; } = new ITableInfo[] {};

        public void Discover(IDbConnection connection)
        {
            if (_alreadyDiscovered) return;
            _alreadyDiscovered = true;

            Name = Name ?? connection
                       .CreateCommand("SELECT DATABASE()")
                       .ExecuteScalar() as string;

            Tables = connection
                .CreateCommand(
                    @"  SELECT 
                            TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE 
                            TABLE_SCHEMA=DATABASE() 
                        AND TABLE_TYPE='BASE TABLE'")
                .ExecuteEnumerable((r) =>
                    new MySqlTableInfo()
                    {
                        Database = this,
                        Name = r.GetString(0),
                    })
                .ToArray(); // prevents multiple open readers

            foreach (var table in Tables)
            {
                table.Discover(connection);
            }
        }

        public MySqlFetchRequest CreateFetchRequest()
        {
            return new MySqlFetchRequest();
        }
    }
}