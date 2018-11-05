namespace Red.Data.DataAccess
{
    public class FetchPredicate
    {       
        public virtual string Text { get; private set; }
        public virtual object[] Arguments { get; private set; }

        public FetchPredicate(string text, params object[] arguments)
        {
            Text = text;
            Arguments = arguments;
        }
    }
}