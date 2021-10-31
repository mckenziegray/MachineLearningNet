using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExtensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Removes duplicate elements using a custom function for determining duplicates.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable`1 whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>A copy of source with all items with duplicate keys removed.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(x => x.First());
        }

        /// <summary>
        /// Determines if the sequence contains duplicate elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to check for duplicates.</param>
        /// <returns>True if the sequence contains any repeated elements; otherwise, false.</returns>
        public static bool ContainsDuplicates<TSource>(this IEnumerable<TSource> source)
        {
            return source.Count() != source.Distinct().Count();
        }

        /// <summary>
        /// Executes <paramref name="body"/> for each item in <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of item in <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to loop through.</param>
        /// <param name="body">The action to excecute for each iteration.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> body)
        {
            foreach (T item in source)
                body(item);
        }

        /// <summary>
        /// Executes <paramref name="body"/> for each item in <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of item in <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to loop through.</param>
        /// <param name="body">
        /// The action to excecute for each iteration. 
        /// The first parameter represents the current item in the collection. 
        /// The second parameter represents the index of the current item.
        /// </param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> body)
        {
            int i = 0;

            foreach (T item in source)
                body(item, i++);
        }

        public static (IEnumerable<T> LesserSide, IEnumerable<T> GreaterSide) Split<T>(this IEnumerable<T> source, T threshold)
            where T : IComparable<T>
        {
            return Split(source, threshold, (T item) => item);
        }

        public static (IEnumerable<TSource> LesserSide, IEnumerable<TSource> GreaterSide) Split<TSource, TKey>(
            this IEnumerable<TSource> source, TKey threshold, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            List<TSource> lesserSide = new();
            List<TSource> greaterSide = new();

            foreach (TSource item in source)
            {
                if (keySelector(item).CompareTo(threshold) > 0)
                    greaterSide.Add(item);
                else
                    lesserSide.Add(item);
            }

            return (lesserSide, greaterSide);
        }

        public static double GeometricMean(this IEnumerable<double> source)
        {
            return Math.Pow(source.Aggregate((a, b) => a * b), 1 / source.Count());
        }

        public static double Median(this IEnumerable<double> source)
        {
            int count = source.Count();

            double median;

            if (count == 0)
            {
                throw new InvalidOperationException();
            }
            else if (count % 2 == 1)
            {
                median = source.ElementAt(count / 2);
            }
            else
            {
                median = source.Skip((count / 2) - 1).Take(2).Average();
            }

            return median;
        }

        public static double GeothmeticMeandian(IEnumerable<double> numbers)
        {
            double aMean, gMean, median;

            do
            {
                aMean = numbers.Average();
                gMean = numbers.GeometricMean();
                median = numbers.Median();

                numbers = new double[] { aMean, gMean, median };
            } while (
                   Math.Abs(aMean - gMean) <= double.Epsilon
                && Math.Abs(aMean - median) <= double.Epsilon
                && Math.Abs(gMean - median) <= double.Epsilon);

            return numbers.First();
        }
    }
}
