using System;

namespace ML
{
    public static class Utils
    {
        #region Distance Functions
        public static double EuclideanDistance(double[] p1, double[] p2)
        {
            if (p1.Length != p2.Length)
                throw new ArgumentException($"{nameof(p1)} and {nameof(p2)} are not the same size.");

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
            if (p1.Length != p2.Length)
                throw new ArgumentException($"{nameof(p1)} and {nameof(p2)} are not the same size.");

            double distance = 0;
            for (int i = 0; i < p1.Length; ++i)
            {
                distance += Math.Abs(p1[i] - p2[i]);
            }

            return distance;
        }
        #endregion
    }
}
