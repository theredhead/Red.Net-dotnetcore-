using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Database {Name}")]
    public class MySqlDatabase : Database
    {
        protected MySqlDatabase() : base(new MySqlDialect())
        {
        }

        public MySqlDatabase(string connectionString) : base(connectionString, new MySqlDialect())
        {
        }

        public override IDbConnection CreateConnection()
        {
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
        public override void Discover()
        {
            Name = Name ?? CreateConnection()
                       .CreateCommand("SELECT DATABASE()")
                       .ExecuteScalar() as string;

            using (var reader = CreateConnection()
                .CreateCommand(
                    @"  SELECT 
                            TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE 
                            TABLE_SCHEMA=DATABASE() 
                        AND TABLE_TYPE='BASE TABLE'")
                   .ExecuteReader())
            {
                while(reader.Read())
                {
                    var name = reader.GetString(0);
                    var table = new MySqlTableInfo()
                    {
                        Database = this,
                        Name = name
                    };
                    table.Discover(CreateConnection());
                    _Tables.Add(name, table);
                }
            }
        }
    }
}