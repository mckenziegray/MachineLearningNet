using System;
using System.Linq;

namespace DotNetExtensions
{
    public static class CharExtensions
    {
        public static bool IsRomanNumeral(this char source, bool allowLower = true)
        {
            char[] romanNumerals = { 'I', 'V', 'X', 'L', 'C', 'D', 'M' };

            return romanNumerals.Contains(allowLower ? char.ToUpper(source) : source);
        }
    }
}
