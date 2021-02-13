using System;
using System.Collections.Generic;
using System.Text;

namespace ML.Data
{
    public class LabelledData<T> : Data
    {
        public T[] Labels { get; set; }

        public LabelledData(double[][] features, T[] labels)
            : base(features)
        {
            Labels = labels;
            if (Labels.Length != RowCount)
                throw new ArgumentException($"The number of data rows is not equal to the number of labels. Rows: {RowCount}; Labels: {Labels.Length}");
        }
    }
}
