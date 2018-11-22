using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Red.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Provides a way to work with embedded placeholders in strings
    /// </summary>
    public class Replacements : Dictionary<string, string>
    {
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        
        public Replacements()
        {
        }

        public Replacements(Dictionary<string,string> replacements) : this()
        {
            foreach (var replacement in replacements)
            {
                Add(replacement.Key, replacement.Value);
            }
        }

        public Replacements(string[] placeholders, string[] values) : this()
        {
            Sanity.Enforce<PlaceholderReplacementCountMismatchException>(placeholders.Length == values.Length);

            for (var index = 0; index < placeholders.Count(); index++)
            {
                Add(placeholders[index], values[index]);
            }
        }

        private string FormatPlaceholder(string name) => $"{Prefix}{name}{Suffix}";
        
        /// <summary>
        /// Apply this collection of replacements to a string containing placeholders.
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <returns></returns>
        public string ApplyTo(string messageFormat)
        {
            var builder = new StringBuilder(messageFormat);

            foreach (var needle in this)
            {
                builder.Replace(FormatPlaceholder(needle.Key), needle.Value);
            }

            return builder.ToString();
        }
    }
}