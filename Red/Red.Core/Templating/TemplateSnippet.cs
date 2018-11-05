namespace Red.Core.Templating
{
    public abstract class TemplateSnippet
    {
        public string Text { get; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }

        public TemplateSnippet(string text, int line, int column)
        {
            Text = text;
            LineNumber = line;
            ColumnNumber = column;
        }
    }
}