using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace Red.Core.Tests
{
    public class TokenizerTests
    {
        private long Time(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();

            action();

            var elapsed = sw.ElapsedMilliseconds;
            sw.Stop();

            return elapsed;
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("Hello, World!",5)]
        [InlineData("foo bar baz", 5)]
        [InlineData("123\n456 789", 5)]
        public void Test1(string input, int expectedNumberOfTokens)
        {
            var scanner = new StringScanner(input);
            var tokenizer = new Tokenizer(scanner);
            var tokens = tokenizer.Tokenize().ToArray();

            Assert.Equal(expectedNumberOfTokens, tokens.Length);
        }

        [Fact]
        public void CanParseSourceFile()
        {
            const string path = "/Users/kris/Projects/Red.Net/Red/Red.Core/Tokenizer.cs";
            var input = File.ReadAllText(path);

            Token[] tokens = null;
            var elapsed = Time(() => {
                var scanner = new StringScanner(input);
                var tokenizer = new Tokenizer(scanner);
                tokens = tokenizer.Tokenize().ToArray();
            });
            Assert.InRange(elapsed, 1, 1000);
            Assert.True(10 < tokens.Length);
        }
    }
}
