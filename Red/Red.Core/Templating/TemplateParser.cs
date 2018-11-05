using System.Collections.Generic;

namespace Red.Core.Templating
{
    public class TemplateParser
    {
        private const string OpenCodeToken = "<#";
        private const string CloseCodeToken = "#>";

        private string Input { get; }
        private int index = 0;
        private int LineNumber = 0;
        private int ColumnNumber = 0;

        private bool IsToken(string token)
        {
            var length = token.Length;
            if ((index + length) > Input.Length)
                return false;

            return Input.Substring(index, length) == token;
        }

        public TemplateParser(string template)
        {
            Input = template;
        }

        private List<TemplateSnippet> Snippets { get; } = new List<TemplateSnippet>();
        
        public Template Parse()
        {
            do
            {
                var snippet = IsToken(OpenCodeToken)
                    ? ReadCodeSnippet()
                    : ReadTextSnippet();

                Snippets.Add(snippet);
            } while (!AtEnd());

            return 
                new Template()
                {
                    Snippets = Snippets.ToArray() 
                    
                };
        }

        private bool AtEnd()
        {
            return index > Input.Length - 1;
        }

        private void MoveToNextCharacter()
        {
            switch (Input[index])
            {
                case '\n':
                    ColumnNumber = 0;
                    LineNumber++;
                    break;
                default:
                    ColumnNumber++;
                    break;
            }
            index++;
        }

        private string ReadUntil(string token)
        {
            var startIndex = index;

            while (!AtEnd() && !IsToken(token))
            {
                MoveToNextCharacter();
            }

            var result = Input.Substring(startIndex, index - startIndex);
            return result;
        }
        
        private TemplateSnippet ReadCodeSnippet()
        {
            var line = LineNumber;
            var column = ColumnNumber;
            
            var content = ReadUntil(CloseCodeToken);

            foreach (var c in  CloseCodeToken.ToCharArray())
                MoveToNextCharacter();

            content += CloseCodeToken;
            
            return new CodeSnippet(content, line, column);
        }

        private TemplateSnippet ReadTextSnippet()
        {
            var line = LineNumber;
            var column = ColumnNumber;

            var content = ReadUntil(OpenCodeToken);
            return new TextSnippet(content, line, column);
        }
    }
}