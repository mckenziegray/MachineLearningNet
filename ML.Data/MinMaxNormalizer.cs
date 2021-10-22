using DotNetExtensions;

namespace ML.Data
{
    /// <summary>
    /// Implementation of <see cref="IDataNormalizer"/> which normalizes by scaling the max and min in each column to 1 and 0, respectively.
    /// </summary>
    public class MinMaxNormalizer : IDataNormalizer
    {
        public Data Normalize(Data data)
        {
            return new Data(Normalize(data.Features));
        }

        public Matrix<double> Normalize(Matrix<double> data)
        {
            double[] mins = new double[data.ColumnCount];
            for (int i = 0; i < mins.Length; ++i)
            {
                mins[i] = double.PositiveInfinity;
            }

            double[] maxes = new double[data.ColumnCount];
            for (int i = 0; i < maxes.Length; ++i)
            {
                maxes[i] = double.NegativeInfinity;
            }

            foreach (double[] row in data)
            {
                for (int i = 0; i < data.ColumnCount; i++)
                {
                    if (row[i] < mins[i])
                        mins[i] = row[i];

                    if (row[i] > maxes[i])
                        maxes[i] = row[i];
                }
            }

            Matrix<double> normalizedData = new(data.RowCount, data.ColumnCount);

            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    normalizedData[i][j] = (data[i][j] - mins[j]) / (maxes[j] - mins[j]);
                }
            }

            return normalizedData;
        }
    }
}
