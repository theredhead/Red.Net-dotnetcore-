using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess.Sqlite
{
    [DebuggerDisplay("SqliteDatabase {Name}")]
    public class SqliteDatabase : Database
    {
        public SqliteDatabase(string connectionString) : base(connectionString, new SqliteDialect())
        {
            ExtensionMethodsOnIDbConnection.ParameterFormattingFunc = 
                ExtensionMethodsOnIDbConnection.PrefixWithDollarSign;
        }

        public override IDbConnection CreateConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public override void Discover()
        {
            _Tables.Clear();

            using (var connection = CreateConnection())
            {
                using (var reader = connection.CreateCommand("SELECT name FROM sqlite_master WHERE type='table';").ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var table = new SqliteTableInfo()
                        {
                            Database = this,
                            Name = name
                        };
                        table.Discover();
                        _Tables.Add(name.ToLowerInvariant(), table);
                    }
                }
            }
        }
    }

    [DebuggerDisplay("SqliteTableInfo {Name}")]
    public class SqliteTableInfo : TableInfo
    {
        internal void Discover()
        {
            var commandText = $"PRAGMA table_info({Name})";
            using (var connection = Database.CreateConnection())
            {
                using (var command = connection.CreateCommand(commandText))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var name = reader.GetString(1);
                            var column = new SqliteColumnInfo()
                            {
                                Table = this,
                                Name = name,
                                DataType = Database.Dialect.GetDbTypeFromString(reader.GetString(2)),
                                IsNullable = ! reader.GetBoolean(3)
                            };
                            _Columns.Add(name.ToLowerInvariant(), column);
                        }
                    }
                }
            }
        }
    }

    [DebuggerDisplay("SqliteColumnInfo {Name}")]
    public class SqliteColumnInfo : ColumnInfo
    {

    }
}