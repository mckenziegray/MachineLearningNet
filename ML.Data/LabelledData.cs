using System;
using System.Collections.Generic;
using System.Linq;
using DotNetExtensions;
using DotNetExtensions.Models;

namespace ML.Data
{
    /// <summary>
    /// A set of numeric data with labels of any type
    /// </summary>
    /// <typeparam name="T">The data type of the labels</typeparam>
    public class LabelledData<T>
    {
        public LabelledMatrix<double, T> Rows { get; init; }

        public Matrix<double> Features => Rows.Data;

        public T[] Labels => Rows.Labels;

        public IEnumerable<T> AllLabels { get; init; }

        /// <summary>
        /// Constructs a set of data using the given features and labels. 
        /// The length of <paramref name="labels"/> must be equal to the number of rows in <paramref name="features"/>.
        /// </summary>
        /// <param name="features">A <see cref="Matrix{T}"/> containing the numeric data features.</param>
        /// <param name="labels">An array containing the labels for the data.</param>
        public LabelledData(Matrix<double> features, T[] labels)
        {
            Rows = new LabelledMatrix<double, T>(features, labels);
            AllLabels = labels.Distinct();
        }
    }
}
