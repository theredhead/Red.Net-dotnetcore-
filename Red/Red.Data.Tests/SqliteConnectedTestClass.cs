using Red.Data.DataAccess.Sqlite;

namespace Red.Data.Tests
{
    public abstract class SqliteConnectedTestClass
    {
        public SqliteDatabase Database { get; }

        public SqliteConnectedTestClass()
        {
            Database = new SqliteDatabase("Data Source=chinook.db;Version=3;");
            Database.Discover();
        }
    }
}