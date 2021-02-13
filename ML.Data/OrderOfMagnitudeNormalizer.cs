using DotNetExtensions;

namespace ML.Data
{
    public class OrderOfMagnitudeNormalizer : IDataNormalizer
    {
        protected double K { get; set; }

        public OrderOfMagnitudeNormalizer(double k)
        {
            K = k;
        }

        public Data Normalize(Data data)
        {
            return Normalize(data.Features);
        }

        public Data Normalize(Matrix<double> data)
        {
            Data normalizedData = new Data(new Matrix<double>(data.RowCount, data.ColumnCount));

            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    normalizedData.Features[i][j] = data[i][j] / K;
                }
            }

            return normalizedData;
        }
    }
}
