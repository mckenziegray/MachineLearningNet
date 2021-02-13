using System;
using DotNetExtensions;

namespace ML.Data
{
    public class LabelledData<T> : Data
    {
        public T[] Labels { get; set; }

        public LabelledData(Matrix<double> features, T[] labels)
            : base(features)
        {
            Labels = labels;

            if (Labels.Length != features.RowCount)
                throw new ArgumentException($"The number of data rows is not equal to the number of labels. Rows: {features.RowCount}; Labels: {Labels.Length}");
        }
    }
}
