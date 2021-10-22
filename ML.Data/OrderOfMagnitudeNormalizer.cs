using DotNetExtensions;

namespace ML.Data
{
    /// <summary>
    /// Implementation of <see cref="IDataNormalizer"/> which normalizes by dividing by a scalar, k.
    /// </summary>
    public class OrderOfMagnitudeNormalizer : IDataNormalizer
    {
        protected double K { get; set; }

        public OrderOfMagnitudeNormalizer(double k)
        {
            K = k;
        }

        public Data Normalize(Data data)
        {
            return new Data(Normalize(data.Features));
        }

        public Matrix<double> Normalize(Matrix<double> data)
        {
            Matrix<double> normalizedData = new(data.RowCount, data.ColumnCount);

            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    normalizedData[i][j] = data[i][j] / K;
                }
            }

            return normalizedData;
        }
    }
}
