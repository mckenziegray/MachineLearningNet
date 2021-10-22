using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetExtensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string source)
        {
            return source switch
            {
                null => null,
                "" => string.Empty,
                _ => source.First().ToString().ToUpper() + source[1..].ToLower()
            };
        }

        public static string CapitalizeAll(this string source)
        {
            return source switch
            {
                null => null,
                _ => Regex.Replace(source, @"(?<=[\s-]\p{L})", m => m.Value.ToUpper())
            };
        }

        public static string ReplaceAll(this string source, IList<string> oldValues, IList<string> newValues)
        {
            if (oldValues is null)
                throw new ArgumentNullException(nameof(oldValues));
            if (newValues is null)
                throw new ArgumentNullException(nameof(newValues));
            if (oldValues.Count != newValues.Count)
                throw new ArgumentException($"{nameof(oldValues)} and {nameof(newValues)} must be the same size. {nameof(oldValues)}: {oldValues.Count}; {nameof(newValues)}: {newValues.Count}.");

            Dictionary<string, string> replacements = new();
            for (int i = 0; i < oldValues.Count; ++i)
                replacements.Add(oldValues[i], newValues[i]);

            return source.ReplaceAll(replacements);
        }

        public static string ReplaceAll(this string source, IDictionary<string, string> replacements)
        {
            if (replacements is null)
                throw new ArgumentNullException(nameof(replacements));

            return Regex.Replace(source, string.Join('|', replacements.Keys.Select(k => Regex.Escape(k))), m => replacements[m.Value]);
        }

        public static bool IsRomanNumeral(this string source)
        {
            return source.Any() && source.All(c => c.IsRomanNumeral());
        }

        public static string ToString(this string source, NameDisplayType displayType)
        {
            return displayType switch
            {
                NameDisplayType.Default => source,
                NameDisplayType.FullCapitalized => source.CapitalizeAll(),
                NameDisplayType.FullUpper => source.ToUpper(),
                NameDisplayType.FullLower => source.ToLower(),
                NameDisplayType.InitialUpper => source.First().ToString().ToUpper(),
                NameDisplayType.InitialLower => source.First().ToString().ToLower(),
                NameDisplayType.InitialDotUpper => $"{char.ToUpper(source.First())}.",
                NameDisplayType.InitialDotLower => $"{char.ToLower(source.First())}.",
                NameDisplayType.None or _ => string.Empty
            };
        }
    }
}
