using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetExtensions;

namespace ML.Data
{
    /// <summary>
    /// Implementation of <see cref="IDataNormalizer"/> which normalizes by mulitplying by the z-score (mean divided by standard deviation)
    /// </summary>
    public class ZScoreNormalizer : IDataNormalizer
    {
        public Data Normalize(Data data)
        {
            return new Data(Normalize(data.Features));
        }

        public Matrix<double> Normalize(Matrix<double> data)
        {
            double[] means = new double[data.ColumnCount];
            for (int i = 0; i < data.ColumnCount; ++i)
            {
                means[i] = 0;
            }
            foreach (double[] row in data)
            {
                for (int i = 0; i < data.ColumnCount; i++)
                {
                    means[i] += row[i];
                }
            }
            means = means.Select(s => s / data.RowCount).ToArray();

            double[] stdDevs = new double[data.ColumnCount];
            foreach (double[] row in data)
            {
                for (int i = 0; i < data.ColumnCount; i++)
                {
                    double diff = row[i] - means[i];
                    stdDevs[i] += diff * diff;
                }
            }
            stdDevs = stdDevs.Select(s => Math.Sqrt(s / data.RowCount)).ToArray();

            Matrix<double> normalizedData = new Matrix<double>(data.RowCount, data.ColumnCount);

            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    normalizedData[i][j] = (data[i][j] - means[j]) / stdDevs[j];
                }
            }

            return normalizedData;
        }
    }
}
