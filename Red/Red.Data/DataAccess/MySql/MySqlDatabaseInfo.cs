using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using MySql.Data.MySqlClient;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess.MySql
{
    [DebuggerDisplay("Database {Name}")]
    public class MySqlDatabaseInfo : Database
    {
        public MySqlDatabaseInfo(ISqlDialect dialect) : base(dialect)
        {
        }

        public MySqlDatabaseInfo(string connectionString, ISqlDialect dialect) : base(connectionString, dialect)
        {
        }

        public override IDbConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
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