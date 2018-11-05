using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Red.Core.Templating
{
    public class TemplateRunner
    {
        public string Namespace { get; set; } = "Generated";
        public string ClassName { get; set; } = "Template";

        private const string IndentChars = "\t";

        private int Indent = 1;

        private StringBuilder builder = new StringBuilder();

        private string CurrentIndent => string.Join(IndentChars, Enumerable.Range(0, Indent).Select(s=>"").ToArray());

        protected virtual void Write(string text)
        {
            builder.Append(text);
        }

        protected virtual void WriteLine(string text)
        {
            Write(text);
            Write("\n");
        }
        
        protected virtual string Run(Template template)
        {
            foreach (var snippet in template.Snippets)
            {
                AppendSnippet(snippet);
            }

            return builder.ToString();
        }

        public string Execute(Template template)
        {
            var code = Run(template);
            var output = ConsoleHelper.CaptureConsoleOutput(() =>
            {
                object result = null;
                CSharpScript.EvaluateAsync(code)
                    .ContinueWith(s => result = s.Result).Wait();
            });

            return output;
        }

        private void AppendSnippet(TemplateSnippet snippet)
        {
            if (snippet is CodeSnippet codeSnippet)
            {
                Write(CurrentIndent);
                WriteLine($"// line #{snippet.LineNumber}");

                if (codeSnippet.Code.StartsWith("="))
                {
                    var expression = codeSnippet.Code.Substring(1);
                    Write(CurrentIndent);
                    WriteLine($"Console.Write({expression});");
                }
                else
                {
                    Write(CurrentIndent);
                    WriteLine(codeSnippet.Code);
                }
            }
            else
            {
                if (snippet.Text.Trim() != "")
                {
                    Write(CurrentIndent);
                    WriteLine($"// line #{snippet.LineNumber}");
                    
                    Write(CurrentIndent);
                    Write("Console.Write(\"");
    
                    Write(Sanitize(snippet.Text));
    
                    WriteLine("\");");
                    WriteLine("");
                }
            }
        }

        private string Sanitize(string text)
        {
            return text
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }
    }
}