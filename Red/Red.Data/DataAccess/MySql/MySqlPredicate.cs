namespace Red.Data.DataAccess.MySql
{
    public class MySqlPredicate : FetchPredicate
    {
        public MySqlPredicate(string text, object[] arguments) : base(text, arguments)
        {
        }
    }
}