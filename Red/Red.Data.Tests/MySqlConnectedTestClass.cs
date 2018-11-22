using System.Data;
using MySql.Data.MySqlClient;

namespace Red.Data.Tests
{
    public abstract class MySqlConnectedTestClass : BaseTestClass
    {
        private const string ConnectionString = "Server=db.theredhead.nl;Database=MyDataProject;Uid=www;Pwd=www;";
        protected IDbConnection CreateConnection()
        {
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
