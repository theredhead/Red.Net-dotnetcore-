using System.Data;
using MySql.Data.MySqlClient;

namespace Red.Data.Tests
{
    public abstract class MySqlConnectedTestClass : BaseTestClass
    {
        private const string ConnectionString = "Server=db.theredhead.nl;Database=MyDataProject;Uid=www;Pwd=www;";
        private IDbConnection _connection;
        protected IDbConnection GetConnection() => new MySqlConnection(ConnectionString);
    }
}
