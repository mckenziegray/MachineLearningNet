using System;
using System.Linq;

namespace DotNetExtensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Creates an instance of a given type.
        /// </summary>
        /// <typeparam name="T">The type of object to instantiate.</typeparam>
        /// <param name="source">A <see cref="Type"/> object representing the type to instantiate.</param>
        /// <param name="parameters">The parameters to pass to the constructor. Pass no parameters to call an empty constructor.</param>
        /// <returns>An object instance of the given type.</returns>
        public static T Instantiate<T>(this Type source, params object[] parameters)
        {
            try
            {
                return (T)source.GetConstructor(parameters.Select(p => p.GetType()).ToArray()).Invoke(parameters);
            }
            catch
            {
                throw;
            }
        }

        public static bool Is(this Type source, Type other)
        {
            return source == other || source.IsSubclassOf(other);
        }
    }
}
