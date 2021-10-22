using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetExtensions
{
    public interface IRange<T> : IEnumerable<T>
    {
        public T Max { get; set; }

        public T Min { get; set; }

        public int Count { get; }
    }
}
