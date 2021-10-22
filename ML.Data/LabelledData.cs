using System;
using DotNetExtensions;

namespace ML.Data
{
    /// <summary>
    /// A set of numeric data with labels of any type
    /// </summary>
    /// <typeparam name="T">The data type of the labels</typeparam>
    public class LabelledData<T> : Data
    {
        /// <summary>
        /// The array of labels. 
        /// The label at a given index corresponds to the row in <see cref="Data.Features"/> of the same index.
        /// The length of this array is always the same as the number of rows in of <see cref="Data.Features"/>.
        /// </summary>
        public T[] Labels { get; set; }

        /// <summary>
        /// Constructs a set of data using the given features and labels. 
        /// The length of <paramref name="labels"/> must be equal to the number of rows in <paramref name="features"/>.
        /// </summary>
        /// <param name="features">A <see cref="Matrix{T}"/> containing the numeric data features.</param>
        /// <param name="labels">An array containing the labels for the data.</param>
        public LabelledData(Matrix<double> features, T[] labels)
            : base(features)
        {
            Labels = labels;

            if (Labels.Length != features.RowCount)
                throw new ArgumentException($"The number of data rows is not equal to the number of labels. Rows: {features.RowCount}; Labels: {Labels.Length}");
        }
    }
}
