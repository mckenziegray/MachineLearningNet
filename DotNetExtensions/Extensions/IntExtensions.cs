using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetExtensions
{
    public static class IntExtensions
    {
        public static string ToSignedString(this int source)
        {
            return source < 0 ? source.ToString() : $"+{source}";
        }
    }
}
