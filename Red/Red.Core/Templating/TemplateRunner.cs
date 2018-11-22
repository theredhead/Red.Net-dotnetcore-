using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Red.Core.Console;

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

        public class TemplateArguments<T>
        {
            public StringWriter Output = new StringWriter();
            public T Arguments;
            public TemplateArguments(T arguments)
            {
                Arguments = arguments;
            }
        }
        public string Execute(Template template)
        {
            return Execute<object>(template, null);
        }

        public string Execute<T>(Template template, T arguments)
        {
            // ReSharper disable once UnusedVariable
            return Execute(template, arguments, out var ignored);
        }
        
        public string Execute<T>(Template template, T arguments, out object result)
        {
            var code = Run(template);

            result = null;
            object executionResult = null;
            var globals = new TemplateArguments<T>(arguments);

            var output = ConsoleHelper.CaptureConsoleOutput(() =>
            {
                var options = ScriptOptions.Default.WithReferences(
                    typeof(System.Console).Assembly,
                    typeof(ConsoleHelper).Assembly
                );

                CSharpScript.EvaluateAsync(code, options, globals)
                    .ContinueWith(s => executionResult = s.Result).Wait();
            });
            result = executionResult;
            return globals.Output.ToString();
        }

        private void AppendSnippet(TemplateSnippet snippet)
        {
            if (snippet is CodeSnippet codeSnippet)
            {
                Write(CurrentIndent);
                WriteLine($"// line #{snippet.LineNumber}");

                if (codeSnippet.Code.StartsWith("=", System.StringComparison.Ordinal))
                {
                    var expression = codeSnippet.Code.Substring(1);
                    Write(CurrentIndent);
                    WriteLine($"Output.Write({expression});");
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
                    Write("Output.Write(\"");
    
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