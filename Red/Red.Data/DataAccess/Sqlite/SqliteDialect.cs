using System.Data;

namespace Red.Data.DataAccess.Sqlite
{
    public class SqliteDialect : DbDialect
    {
        public override string QuoteName(string name)
        {
            return $"\"{name}\"";
        }

        protected override FetchPredicate CreatePredicate(string text, params object[] arguments)
        {
            return new FetchPredicate(text, arguments);
        }
    }
}