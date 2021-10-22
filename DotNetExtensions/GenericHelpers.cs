using System;

namespace DotNetExtensions
{
    public static class GenericHelpers
    {
        /// <summary>
        /// Generic method for determining equality using the Equals method of the specified type in a null-safe way.
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns></returns>
        public static bool AreEqual<T>(T a, T b)
        {
            return a is null ? b is null : a.Equals(b);
        }

        /// <summary>
        /// Executes a for loop.
        /// </summary>
        /// <param name="initialIndex">Initial value for the loop variable.</param>
        /// <param name="condition">
        /// A <see cref="Predicate{int}"/> to check on each loop, where the parameter is the loop variable. 
        /// The loop ends when the condition is not true.
        /// </param>
        /// <param name="stepOperation">The operation to execute on the loop variable on each loop.</param>
        /// <param name="body">The operation to perform on each loop.</param>
        public static void For(int initialIndex, Predicate<int> condition, Action<int> stepOperation, Action<int> body)
        {
            for (int i = initialIndex; condition(i); stepOperation(i))
            {
                body(i);
            }
        }

        /// <summary>
        /// Executes a typical for loop, where the loop variable is initialized to 0, 
        /// the step operation increments the loop variable, 
        /// and the loop ends when the variable reaches an upper bound.
        /// </summary>
        /// <param name="upperBound">
        /// When the loop variable reaches this value, the loop will end.
        /// This value will also be equal to the total number of loops if <paramref name="inclusiveBound"/> is false.
        /// </param>
        /// <param name="body">The operation to execute on each loop.</param>
        /// <param name="inclusiveBound">
        /// If true, continues looping until the loop variable is greater than <paramref name="upperBound"/>.
        /// If false, continues looping until the loop variable is equal to <paramref name="upperBound"/>.
        /// Defaults to false.
        /// </param>
        public static void BasicFor(int upperBound, Action<int> body, bool inclusiveBound = false)
        {
            For(0, (int i) => inclusiveBound ? i <= upperBound : i < upperBound, (int i) => ++i, body);
        }
    }
}
