using System;
using System.Collections.Generic;

namespace DotNetExtensions
{
    public static class ComparerExtensions
    {
        public static Comparer<T> Reversed<T>(this IComparer<T> source)
        {
            return Comparer<T>.Create(new Comparison<T>((T a, T b) => source.Compare(b, a)));
        }
    }
}
