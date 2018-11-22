using Red.Data.DataAccess.MySql;
using Xunit;

namespace Red.Data.Tests
{
    public class DatabaseTests : MySqlConnectedTestClass
    {
        [Fact]
        public void Test_That_The_Database_Can_Be_ReverseEngineered()
        {
            var db = new MySqlDatabase("server=192.168.1.106;uid=www;pwd=e17a725e163b6f2beaffa3169f6708ef;database=MyDataProject");
            db.Discover();

            bool haveTables = false;
            Assert.All(db.Tables, (table) => {
                Assert.NotEmpty(table.Name);
                Assert.NotEmpty(table.Columns);

                haveTables = true;
            });
            Assert.True(haveTables);
        }
    }
}
