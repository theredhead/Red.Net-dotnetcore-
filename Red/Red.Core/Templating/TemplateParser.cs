using System.Collections.Generic;

namespace Red.Core.Templating
{
    public class TemplateParser
    {
        private const string OpenCodeToken = "<#";
        private const string CloseCodeToken = "#>";

        private string Input { get; }
        private int _index;
        private int _lineNumber;
        private int _columnNumber;

        private bool IsToken(string token)
        {
            var length = token.Length;
            if ((_index + length) > Input.Length)
                return false;

            return Input.Substring(_index, length) == token;
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
            return _index > Input.Length - 1;
        }

        private void MoveToNextCharacter()
        {
            switch (Input[_index])
            {
                case '\n':
                    _columnNumber = 0;
                    _lineNumber++;
                    break;
                default:
                    _columnNumber++;
                    break;
            }
            _index++;
        }

        private string ReadUntil(string token)
        {
            var startIndex = _index;

            while (!AtEnd() && !IsToken(token))
            {
                MoveToNextCharacter();
            }

            var result = Input.Substring(startIndex, _index - startIndex);
            return result;
        }
        
        private TemplateSnippet ReadCodeSnippet()
        {
            var line = _lineNumber;
            var column = _columnNumber;
            
            var content = ReadUntil(CloseCodeToken);

            foreach (var c in CloseCodeToken.ToCharArray())
                MoveToNextCharacter();

            content += CloseCodeToken;
            
            return new CodeSnippet(content, line, column);
        }

        private TemplateSnippet ReadTextSnippet()
        {
            var line = _lineNumber;
            var column = _columnNumber;

            var content = ReadUntil(OpenCodeToken);
            return new TextSnippet(content, line, column);
        }
    }
}