using System;
using DotNetExtensions;

namespace ML.Data
{
    /// <summary>
    /// A set of unlabelled numeric data
    /// </summary>
    public class Data
    {
        /// <summary>
        /// A <see cref="Matrix{double}"/> containing the data set
        /// </summary>
        public Matrix<double> Features { get; protected set; }

        public Data(double[][] features)
        {
            Features = features;
        }

        public Data(double[,] features)
        {
            Features = new Matrix<double>(features);
        }

        public Data(Matrix<double> features)
        {
            Features = features;
        }

        /// <summary>
        /// Converts a data set to a <see cref="LabelledData{T}" /> by using one of the existing columns for labeling.
        /// </summary>
        /// <typeparam name="T">The data type of the label.</typeparam>
        /// <param name="labelColumnIndex">The index of the column to be used for labeling.</param>
        /// <returns>The <see cref="LabelledData{T}"/>.</returns>
        public LabelledData<T> ToLabelledData<T>(int labelColumnIndex)
        {
            if (labelColumnIndex < 0 || labelColumnIndex >= Features.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(labelColumnIndex));

            T[] labels = new T[Features.RowCount];

            for (int i = 0; i < Features.RowCount; ++i)
            {
                try
                {
                    labels[i] = (T)Convert.ChangeType(Features[i][labelColumnIndex], typeof(T));
                }
                catch (InvalidCastException e)
                {
                    throw new ArgumentException($"The specified data column ({labelColumnIndex}) cannot be converted to {typeof(T)}.", nameof(labelColumnIndex), e);
                }

                double[] x = new double[Features.ColumnCount - 1];
                Array.Copy(Features[i], 0, x, 0, labelColumnIndex);
                Array.Copy(Features[i], labelColumnIndex + 1, x, labelColumnIndex, Features.ColumnCount - 1 - labelColumnIndex);
                Features[i] = x;
            }

            return new LabelledData<T>(Features, labels);
        }

        /// <summary>
        /// Converts a data set to a <see cref="LabelledData{T}" /> using the given array of labels.
        /// </summary>
        /// <typeparam name="T">The data type of the labels.</typeparam>
        /// <param name="labels">An array of labels.</param>
        /// <returns>A set of data containing the features of this data set mapped to the given array of labels.</returns>
        public LabelledData<T> ToLabelledData<T>(T[] labels)
        {
            return new LabelledData<T>(Features, labels);
        }
    }
}
