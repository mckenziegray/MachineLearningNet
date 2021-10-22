using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExtensions
{
    public class Range : IRange<int>
    {
        private readonly bool INCLUSIVE;

        public int Min { get; set; }

        public int Max { get; set; }

        public int Count => INCLUSIVE ? Max - Min + 1 : Max - Min;

        public Range(int max, bool inclusive = false)
            : this(0, max, inclusive)
        { }

        public Range(int min, int max, bool inclusive = false)
        {
            Min = min;
            Max = max;
            INCLUSIVE = inclusive;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return Enumerable.Range(Min, Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.Range(Min, Count).GetEnumerator();
        }
    }
}
