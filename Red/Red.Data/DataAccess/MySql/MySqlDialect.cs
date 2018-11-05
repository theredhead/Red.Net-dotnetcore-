using System.Data;

namespace Red.Data.DataAccess.MySql
{
    public class MySqlDialect : DbDialect
    {
        public override string QuoteName(string name) => $"`{name}`";

        protected override FetchPredicate CreatePredicate(string text, params object[] arguments)
        {
            return new MySqlPredicate(text, arguments);
        }

        public override DbType GetDbTypeFromString(string dataTypeName)
        {
            switch (dataTypeName.ToUpperInvariant())
            {
                case "VARCHAR":
                    return DbType.AnsiString;
                case "CHAR":
                    return DbType.AnsiStringFixedLength;

                case "TINYINT":
                    return DbType.Byte;
                case "SMALLINT":
                    return DbType.Int16;
                case "INT":
                    return DbType.Int32;
                case "BIGINT":
                    return DbType.Int64;

                case "VARBINARY":
                    return DbType.Binary;

                case "DATETIME":
                    return DbType.DateTime;

                default:
                    return DbType.Object;
            }

        }

        public override IDatabaseInfo ReverseEngineer(IDbConnection connection)
        {
            var database = new MySqlDatabaseInfo()
            {
                Dialect = this
            };

            database.Discover(connection);
            return database;
        }
    }
}