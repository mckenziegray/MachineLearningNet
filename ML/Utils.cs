using System;

namespace ML
{
    public static class Utils
    {
        public static int GetNumColumns<T>(T[][] matrix)
        {
            int numColumns = matrix[0].Length;
            for (int i = 1; i < matrix.Length; ++i)
                if (matrix[i].Length != numColumns)
                    throw new ArgumentException($"The array does not have a uniform number of columns.");

            return numColumns;
        }

        #region Distance Functions
        public static double EuclideanDistance(double[] p1, double[] p2)
        {
            double sum = 0;
            for (int i = 0; i < p1.Length; ++i)
            {
                double difference = p1[i] - p2[i];
                sum += difference * difference;
            }

            return Math.Sqrt(sum);
        }

        public static double ManhattanDistance(double[] p1, double[] p2)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
