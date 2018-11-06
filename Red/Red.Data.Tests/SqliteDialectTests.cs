using System;
using Red.Data.DataAccess.Sqlite;
using Xunit;

namespace Red.Data.Tests
{
    public class SqliteDialectTests : SqliteConnectedTestClass
    {
        [Fact]
        public void Test_simple_Query()
        {
            var database = new SqliteDatabase("Data Source=chinook.db;Version=3;");
            
            database.Discover();
            
            var request = database["employees"]
                .Where()
                .Between("Birthdate", new DateTime(1960, 01, 01), new DateTime(1970, 01, 01))
                .Contains("Firstname", "S")
                .Request;

            var data = request.FetchDataTable();
            

        }
    }
}