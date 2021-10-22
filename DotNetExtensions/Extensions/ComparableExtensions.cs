using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetExtensions
{
    public static class ComparableExtensions
    {
        /// <summary>
        /// Determines if a value is between two other values.
        /// </summary>
        /// <typeparam name="T">The type of items to compare to.</typeparam>
        /// <param name="source">The item to check for "betweenness".</param>
        /// <param name="start">The inclusive lower bound.</param>
        /// <param name="end">The upper bound. May be inclusive or exclusive based on <paramref name="inclusive"/>.</param>
        /// <param name="inclusive">Whether the upper bound should be inclusive or exclusive.</param>
        /// <returns>
        /// True if <paramref name="source"/> is in the same position or after <paramref name="start"/> 
        /// and either <paramref name="source"/> comes before <paramref name="end"/> or 
        /// <paramref name="inclusive"/> is true and <paramref name="source"/> is in the same position as <paramref name="end"/>.
        /// False otherwise.
        /// </returns>
        public static bool Between<T>(this IComparable<T> source, T start, T end, bool inclusive = false)
        {
            int compValStart = source.CompareTo(start);
            int compValEnd = source.CompareTo(end);

            return compValStart >= 0 && (inclusive ? compValEnd <= 0 : compValEnd < 0);
        }
    }
}
