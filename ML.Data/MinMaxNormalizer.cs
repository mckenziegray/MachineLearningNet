using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Data
{
    public class MinMaxNormalizer : INormalizer
    {
        public Data Normalize(Data data)
        {
            return Normalize(data.Features);
        }

        public Data Normalize(double[][] data)
        {
            if (data.Length > 0 && data.Any(v => v.Length != data[0].Length))
                throw new ArgumentException();

            double[] mins, maxes;


        }
    }
}
