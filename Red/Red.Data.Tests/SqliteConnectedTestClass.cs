using System.Data.SQLite;
using Red.Data.DataAccess.Sqlite;

namespace Red.Data.Tests
{
    public abstract class SqliteConnectedTestClass
    {
        private SQLiteConnection Connection { get; }
        public SqliteDatabase Database { get; set; }

        public SqliteConnectedTestClass()
        {
            Database = new SqliteDatabase("Data Source=chinook.db;Version=3;");
            Database.Discover();
        }
    }
}