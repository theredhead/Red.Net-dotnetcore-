using System;
using System.Data;
using MySql.Data.MySqlClient;
using Red.Data.DataAccess.MySql;

namespace Red.Data.DataAccess.Generation
{
    public class DatabaseProject
    {
        private string ConnectionString { get; }
        private IDbConnection Connection { get; }
        public IDatabaseInfo Database { get; }

        public DatabaseProject(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new MySqlConnection(ConnectionString);
            var dialect = new MySqlDialect();
            Database = dialect.ReverseEngineer(Connection);
        }
    }
}
