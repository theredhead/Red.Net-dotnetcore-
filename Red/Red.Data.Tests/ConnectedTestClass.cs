using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Red.Data.DataAccess.MySql;
using Xunit;

namespace Red.Data.Tests
{
    public class DatabaseTests : MySqlConnectedTestClass
    {
        [Fact]
        public void Test_That_The_Database_Can_Be_ReverseEngineered()
        {
            using (var connection = GetConnection())
            {
                var elapsed = Time(() => {

                    connection.Open();
                    var dialect = new MySqlDialect();

                    var database = dialect.ReverseEngineer(connection);
                     
                    MySqlTableInfo personTbl = (
                        from table in database.Tables
                        where table.Name == "Person"
                        select table).Single() as MySqlTableInfo;

                    var request = personTbl.Search(new[] { "Kris" });
                    var cmd = request.GetCommand(connection, dialect);

                    var tbl = new DataTable();
                    using (var reader = cmd.ExecuteReader())
                    {
                        tbl.Load(reader);
                    }

                    connection.Close();

                });
            }
        }

    }
}
