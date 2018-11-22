using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Red.Core
{
    public class StringScanner
    {
        private readonly string _input;
        private int _index;
        public int LineNumber { get; private set; }
        public int Column { get; private set; }

        public StringScanner(string input)
        {
            _input = input;
        }

        public IEnumerable<char> Scan()
        {
            LineNumber = 1;
            while (_index < _input.Length)
            {
                char character = _input[_index];
                HandleCharacter(character);
                _index++;
                yield return character;
            }
        }

        protected virtual void HandleCharacter(char character)
        {
            switch (character)
            {
                case '\n': Newline(); break;
                default: Column++; break;
            }
        }

        private void Newline()
        {
            Column = 0;
            LineNumber++;
        }
    }

    [DebuggerDisplay("Token: {Content} ({LineNumber},{Column})")]
    public class Token
    {
        public Token(string content, int lineNumber, int column)
        {
            Content = content;
            LineNumber = lineNumber;
            Column = column;
        }

        public string Content { get; set; }
        public int LineNumber { get; set; }
        public int Column { get; set; }
    }

    public class Tokenizer
    {
        public class CharacterInfo
        {
            public int LineNumber { get; set; }
            public int Column { get; set; }
            public char Character { get; internal set; }
        }

        public class Characters : List<CharacterInfo>
        {
            public Token GetTokenAndClear()
            {
                var stringBuilder = new StringBuilder();
                CharacterInfo firstCharacter = null;
                bool isFirstIteration = true;

                foreach (var info in this)
                {
                    if (isFirstIteration)
                    {
                        firstCharacter = info;
                        isFirstIteration = false;
                    }
                    stringBuilder.Append(info.Character);
                }
                if (firstCharacter != null)
                {
                    var result = new Token(
                        stringBuilder.ToString(),
                        firstCharacter.LineNumber,
                        firstCharacter.Column);
                    Clear();
                    return result;
                }
                else
                    throw new Exception("No characters have been scanned yet.");
            }
        }

        StringScanner Scanner;

        public Tokenizer(StringScanner scanner)
        {
            Scanner = scanner;
        }

        protected virtual bool IsLowerAlpha(char c) => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c);
        protected virtual bool IsUpperAlpha(char c) => "abcdefghijklmnopqrstuvwxyz".Contains(c);
        protected virtual bool IsAlpha(char c) => IsLowerAlpha(c) || IsUpperAlpha(c);

        protected virtual bool IsDigit(char c) => "0123456789".Contains(c);
        protected virtual bool IsMath(char c) => "+-/*%^".Contains(c);
        protected virtual bool IsWhitespace(char c) => "\r\n\t ".Contains(c);
        protected virtual bool IsDelimiter(char c) => !IsAlpha(c) && !IsDigit(c);

        protected virtual IEnumerable<CharacterInfo> Scan()
        {
            foreach (char character in Scanner.Scan())
            {
                yield return new CharacterInfo()
                {
                    LineNumber = Scanner.LineNumber,
                    Column = Scanner.Column,
                    Character = character
                };
            }
        }

        public IEnumerable<Token> Tokenize()
        {
            var buffer = new Characters();
            foreach (var characterInfo in Scan())
            {
                if (IsDelimiter(characterInfo.Character) && buffer.Any())
                    yield return buffer.GetTokenAndClear();

                buffer.Add(characterInfo);

                if (IsDelimiter(characterInfo.Character))
                    yield return buffer.GetTokenAndClear();
            }
            if (buffer.Any())
                yield return buffer.GetTokenAndClear();
        }

        protected virtual Token CreateToken(string word, int lineNumber, int column)
        {
            return new Token(word, lineNumber, column);
        }
    }
}
