using System;

namespace Red.Core
{
    public sealed class PlaceholderReplacementCountMismatchException : Exception
    {
        public PlaceholderReplacementCountMismatchException() : base("There is a mismatch between the number of placeholders and replacements")
        {    
        }
    }
}