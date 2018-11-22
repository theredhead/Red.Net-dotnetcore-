using Xunit;

namespace Red.Core.Tests
{
    /// <summary>
    /// Class under Test: <see cref="Red.Core.Replacements"/>
    /// </summary>
    public class ReplacementsTests
    {
        [Fact]
        void Replacements_ApplyTo__Can_Replace_Simple_Words()
        {
            const string sentence = "The quick, brown fox jumps over the lazy dog.";
            const string expected = "The Quick, Brown Fox jumps over the Lazy Dog.";
            
            var replacements = new Replacements
            {
                {"quick", "Quick"},
                {"brown", "Brown"},
                {"fox", "Fox"},
                {"lazy", "Lazy"},
                {"dog", "Dog"}
            };

            Assert.Equal(expected, replacements.ApplyTo(sentence));
        }
        
        [Fact]
        void Replacements_ApplyTo__Can_Interpolate_using_Prefixes_and_Suffixes()
        {
            const string sentence = "The {firstAdjective}, {secondAdjective} {firstCreature} {verb} {position} the {thirdAdjective} {secondCreature}.";
            const string expected = "The Quick, Brown Fox jumps over the Lazy Dog.";
            
            var replacements = new Replacements
            {
                {"firstAdjective", "Quick"},
                {"secondAdjective", "Brown"},
                {"firstCreature", "Fox"},
                {"verb", "jumps"},
                {"position", "over"},
                {"thirdAdjective", "Lazy"},
                {"secondCreature", "Dog"}
            };
            replacements.Prefix = "{";
            replacements.Suffix = "}";
            
            Assert.Equal(expected, replacements.ApplyTo(sentence));
        }

        [Fact]
        void Replacements__Does_not_allow_mismatch_between_Placeholders_and_Values()
        {
            var placeholders = new[] {"1"}; 
            var values = new[] { "1", "2" };
            var exception = Assert.Throws<PlaceholderReplacementCountMismatchException>(() => new Replacements(placeholders, values));

            Assert.NotNull(exception);
        }
    }
}