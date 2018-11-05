using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Red.Core
{
    public static class StringAdditions
    {
        private static readonly char[] NumericCharacters = new []{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        
        public static string NextInSequence(this string aString)
        {
            return aString;
        }

        public static long GetNumericPrefix(this string aString)
        {
            var position = 0;
            foreach (var character in aString)
            {
                if (NumericCharacters.Contains(character))
                    position ++;
                else
                    break;
            }

            var numericPart = aString.Substring(0, position);

            return long.TryParse(numericPart, out var number) ? number : 0;
        }

        public static long GetNumericSuffix(this string aString)
        {
            var position = aString.Length;
            for (var index = aString.Length -1; index > 0; index --)
            {
                var character = aString[index];
                if (NumericCharacters.Contains(character))
                    position--;
                else
                    break;
            }

            var numericPart = aString.Substring(position);

            return long.TryParse(numericPart, out var number) ? number : 0;
        }

        public static string GetMessageDigest(this string message)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(message);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var @byte in hashBytes)
                {
                    sb.Append(@byte.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}