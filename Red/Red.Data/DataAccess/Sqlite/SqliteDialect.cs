namespace Red.Data.DataAccess.Sqlite
{
    public class SqliteDialect : DbDialect
    {
        public override string QuoteName(string name)
        {
            return $"\"{name}\"";
        }
    }
}