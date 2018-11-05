using Xunit;

namespace Red.Core.Tests
{
    public class StringAdditionsTests
    {
        [Theory]
        [InlineData("NoPrefix", 0)]
        [InlineData("0ZeroPrefix", 0)]
        [InlineData("0.ZeroPrefixWithPeriodSeparator", 0)]
        [InlineData("1.PrefixedString", 1)]
        [InlineData("1_PrefixedString", 1)]
        [InlineData("123_PrefixedString", 123)]
        [InlineData("321_PrefixedString", 321)]
        [InlineData("1234567890_PrefixedString", 1234567890)]
        [InlineData("987654321_PrefixedString", 987654321)]
        public void String_GetNumericPrefix__Yields_expected_result(string testString, long expectedPrefix)
        {
            Assert.Equal(expectedPrefix, testString.GetNumericPrefix());
        }
        
        [Theory]
        [InlineData("NoSuffix", 0)]
        [InlineData("ZeroSuffix0", 0)]
        [InlineData("ZeroSuffixWithPeriodSeparator.0", 0)]
        [InlineData("SuffixedString.1", 1)]
        [InlineData("SuffixedString_1", 1)]
        [InlineData("SuffixedString_123", 123)]
        [InlineData("SuffixedString_321", 321)]
        [InlineData("SuffixedString_1234567890", 1234567890)]
        [InlineData("SuffixedString_987654321", 987654321)]
        public void String_GetNumericSuffix__Yields_expected_result(string testString, long expectedSuffix)
        {
            Assert.Equal(expectedSuffix, testString.GetNumericSuffix());
        }

    }
}