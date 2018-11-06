using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Red.Data.DataAccess.Base;

namespace Red.Data.DataAccess.Sqlite
{
    public class SqliteDatabase : Database
    {
        public SqliteDatabase(string connectionString) : base(connectionString, new SqliteDialect())
        {
        }

        public override IDbConnection CreateConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public override void Discover()
        {
            _Tables.Clear();

            using (var connection = CreateConnection())
            {
                using (var reader = connection.CreateCommand("SELECT name FROM sqlite_master WHERE type='table';").ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);

                        _Tables.Add(name.ToLowerInvariant(), new TableInfo()
                        {
                            Database = this,
                            Name = name
                        });
                    }
                }
            }
        }
    }
}