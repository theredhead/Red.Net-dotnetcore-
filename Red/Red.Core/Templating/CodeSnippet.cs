using System;
using System.IO;

namespace Red.Core.Templating
{
    public class CodeSnippet : TemplateSnippet
    {
        public CodeSnippet(string text, int line, int column) : base(text, line, column)
        {
        }

        public string Code => Text.Substring(2, Text.Length - 4);
    }
}