using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Data
{
    public interface INormalizer
    {
        Data Normalize(Data data);

        Data Normalize(double[][] data);
    }
}
