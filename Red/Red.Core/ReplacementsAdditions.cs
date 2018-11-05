using System;
using System.Collections.Generic;
using System.Linq;

namespace Red.Core
{
    /// <summary>
    /// Provides some extension methods to make working with <see cref="Replacements"/> more convenient 
    /// </summary>
    public static class ReplacementsAdditions
    {
        /// <summary>
        /// Adds a quick wat to convert a Dictionary of string mapped objects to a <see cref="ToReplacements"/> instance.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="placeholderPrefix"></param>
        /// <param name="placeholderSuffix"></param>
        /// <returns></returns>
        public static Replacements ToReplacements(
            this Dictionary<string, object> dictionary,
            string placeholderPrefix = null,
            string placeholderSuffix = null)
        {
            var converted = dictionary.ToDictionary(
                (item) => item.Key,
                (item) => Convert.ToString(item.Value));

            var placeholders = new Replacements(converted);
            
            if (placeholderPrefix != null)
                placeholders.Prefix = placeholderPrefix;

            if (placeholderSuffix != null)
                placeholders.Suffix = placeholderSuffix; 
            
            return placeholders;
        }

        /// <summary>
        /// Applies a Dictionary of string mapped objects as Replacements to a given format string, using <see cref="Replacements"/>.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="formatString"></param>
        /// <param name="placeholderPrefix"></param>
        /// <param name="placeholderSuffix"></param>
        /// <returns></returns>
        public static string ApplyTo(
            this Dictionary<string, object> dictionary,
            string formatString,
            string placeholderPrefix = null,
            string placeholderSuffix = null)
        {
            return dictionary.ToReplacements(placeholderPrefix, placeholderSuffix).ApplyTo(formatString);
        }
    }
}