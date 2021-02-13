using System;
using System.Collections.Generic;
using System.Text;

namespace ML.Data
{
    /// <summary>
    /// A set of numeric data
    /// </summary>
    public class Data
    {
        public int RowCount { get; protected set; }
        public int ColumnCount { get; protected set; }
        public double[][] Features { get; protected set; }

        public Data(double[][] features)
        {
            Features = features;
            RowCount = Features.Length;
            ColumnCount = Features[0].Length;
            foreach (double[] x in Features)
                if (x.Length != ColumnCount)
                    throw new ArgumentException($"Data rows are not uniform in size.", nameof(features));
        }

        /// <summary>
        /// Converts a data set to a <see cref="LabelledData{T}" /> by using one of the existing columns for labeling.
        /// </summary>
        /// <typeparam name="T">The data type of the label.</typeparam>
        /// <param name="labelColumnIndex">The index of the column to be used for labeling.</param>
        /// <returns>The <see cref="LabelledData{T}"/>.</returns>
        public LabelledData<T> ToLabelledData<T>(int labelColumnIndex)
        {
            if (labelColumnIndex < 0 || labelColumnIndex >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(labelColumnIndex));

            T[] labels = new T[RowCount];

            for (int i = 0; i < RowCount; ++i)
            {
                try
                {
                    labels[i] = (T)Convert.ChangeType(Features[i][labelColumnIndex], typeof(T));
                }
                catch (InvalidCastException e)
                {
                    throw new ArgumentException($"The specified data column ({labelColumnIndex}) cannot be converted to {typeof(T)}.", nameof(labelColumnIndex), e);
                }

                double[] x = new double[ColumnCount - 1];
                Array.Copy(Features[i], 0, x, 0, labelColumnIndex);
                Array.Copy(Features[i], labelColumnIndex + 1, x, labelColumnIndex, ColumnCount - 1 - labelColumnIndex);
                Features[i] = x;
            }

            return new LabelledData<T>(Features, labels);
        }
    }
}
