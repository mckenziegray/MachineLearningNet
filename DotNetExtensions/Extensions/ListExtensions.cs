using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetExtensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Randomizes the order of the items in the list.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="random">A <see cref="Random"/> object to be used for randomizing the order. A new object will be created if none is provided.</param>
        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            random ??= new Random();

            for (int i = list.Count; i > 0; i--)
            {
                int r = random.Next(0, i);
                T temp = list[0];
                list[0] = list[r];
                list[r] = temp;
            }
        }

        /// <summary>
        /// Creates a string based on the given list.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="separator">The default string used to separate items in the list.</param>
        /// <param name="conjunction">The string to be used to separate the last two items in the list.</param>
        /// <param name="useOxford">Can only be false if <paramref name="conjunction"/> is not null. If true, both the separator and conjunction will be used to separate the last two items in the list if there are more than two items total. Otherwise, only the conjuction will be used to separate the last two items in the list.</param>
        /// <param name="autoSpace">
        ///     Whether spaces should be placed in the string automatically. If true, spaces will be added:
        ///         1. After every separator.
        ///         2. After the conjunction.
        ///         3. Before the conjunction if the conjunction does not follow a separator (because there is already a space after the separator).
        /// </param>
        /// <returns>The list as a string.</returns>
        public static string ToListString<T>(this IList<T> source, string separator, string conjunction = null, bool useOxford = true, bool autoSpace = true)
        {
            if (useOxford == false && conjunction is null)
                throw new NotSupportedException($"{nameof(useOxford)} cannot be false if {nameof(conjunction)} is null.");

            StringBuilder result = new();

            for (int i = 0; i < source.Count; i++)
            {
                bool isLast = i == source.Count - 1;

                if (i != 0)
                {
                    if (conjunction is null || !isLast || (useOxford && source.Count > 2))
                        result.Append(separator);

                    if (autoSpace)
                        result.Append(' ');

                    if (isLast && conjunction is not null)
                    {
                        result.Append(conjunction);
                        if (autoSpace)
                            result.Append(' ');
                    }
                }

                result.Append(source[i]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Creates a string based on the given list.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="separator">The default character used to separate items in the list.</param>
        /// <param name="conjunction">The string to be used to separate the last two items in the list.</param>
        /// <param name="useOxford">Can only be false if <paramref name="conjunction"/> is not null. If true, both the separator and conjunction will be used to separate the last two items in the list if there are more than two items total. Otherwise, only the conjuction will be used to separate the last two items in the list.</param>
        /// <param name="autoSpace">
        ///     Whether spaces should be placed in the string automatically. If true, spaces will be added:
        ///         1. After every separator.
        ///         2. After the conjunction.
        ///         3. Before the conjunction if the conjunction does not follow a separator (because there is already a space after the separator).
        /// </param>
        /// <returns>The list as a string.</returns>
        public static string ToListString<T>(this IList<T> source, char separator, string conjunction = null, bool useOxford = true, bool autoSpace = true)
        {
            return ToListString(source, separator.ToString(), conjunction, useOxford, autoSpace);
        }
    }
}
