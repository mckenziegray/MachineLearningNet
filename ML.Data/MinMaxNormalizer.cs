using DotNetExtensions;

namespace ML.Data
{
    public class MinMaxNormalizer : IDataNormalizer
    {
        public Data Normalize(Data data)
        {
            return Normalize(data.Features);
        }

        public Data Normalize(Matrix<double> data)
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

            Data normalizedData = new Data(new Matrix<double>(data.RowCount, data.ColumnCount));

            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    normalizedData.Features[i][j] = (data[i][j] - mins[j]) / (maxes[j] - mins[j]);
                }
            }

            return normalizedData;
        }
    }
}
