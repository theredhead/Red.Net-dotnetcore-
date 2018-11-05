using System;
using Xunit;
using Red.Core.Templating;
using System.IO;

namespace Red.Core.Tests
{
    public class TemplateParserTests
    {
        [Theory]
        [InlineData("Hello, World!", 1)]
        [InlineData("Hello, <#=planet#>!", 3)]
        [InlineData("<# void #>", 1)]
        public void Text_is_parsed_in_the_expected_number_of_snippets(string text, int parts)
        {
            var parser = new TemplateParser(text);
            var template = parser.Parse();

            Assert.Equal(parts, template.Snippets.Length);
        }

        [Fact]
        public void Executing_Generated_Template_Yields_Expected_Text()
        {
            var rawTemplate = File.ReadAllText("test-template.txt");
            Assert.Equal("9FC5C4C328F9D8B685D68625B42276F1", rawTemplate.GetMessageDigest());
            
            var parser = new TemplateParser(rawTemplate);
            var template = parser.Parse();
            var runner = new TemplateRunner();
            var text = runner.Execute(template);

            Assert.Equal("FED4BB1ECA8ED6C2977800A9EE0B209A", text.GetMessageDigest());
        }
    }
}
