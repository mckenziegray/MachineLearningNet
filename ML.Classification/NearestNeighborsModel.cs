using ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ML.Classification
{
    public class NearestNeighborsModel<T> : IClassificationModel<T>
    {
        protected struct LabeledVector : IComparable<LabeledVector>
        {
            public double[] features;
            public T label;
            public double distance;

            public int CompareTo(LabeledVector other)
            {
                if (distance < other.distance)
                    return -1;
                else if (distance > other.distance)
                    return 1;
                else
                    return 0;
            }
        }

        protected LabeledVector[] Points { get; set; }

        /// <summary>
        /// The number of nearest neighbors to consider during classification.
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// The function to use to calculate the distance between two vectors.
        /// </summary>
        protected Func<double[], double[], double> DistanceFunction { get; set; }

        public NearestNeighborsModel(LabelledData<T> trainingData, int numNeighbors, Func<double[], double[], double> distanceFunction)
            : this(trainingData.Features, trainingData.Labels, numNeighbors, distanceFunction)
        { }

        public NearestNeighborsModel(double[][] features, T[] labels, int numNeighbors, Func<double[], double[], double> distanceFunction)
        {
            int numRows = features.Length;
            if (numRows != labels.Length)
                throw new ArgumentException($"The number of data rows is not equal to the number of labels. Rows: {features.Length}\n Labels: {labels.Length}");
            
            Points = new LabeledVector[numRows];
            for (int i = 0; i < numRows; i++)
            {
                Points[i].features = features[i];
                Points[i].label = labels[i];
            }

            K = numNeighbors;
            DistanceFunction = distanceFunction;
        }

        /// <summary>
        /// Predict the label of a feature vector.
        /// </summary>
        /// <param name="features">The vector of features to classify.</param>
        /// <returns>The predicted label.</returns>
        public T Classify(double[] features)
        {
            // Calculate distance to each data vector
            for (int i = 0; i < Points.Length; ++i)
                Points[i].distance = DistanceFunction(Points[i].features, features);

            // Sort nearest to farthest
            Array.Sort(Points);

            // Calculate the inverse weighted distance for each point
            double[] weights = new double[K];
            for (int i = 0; i < K; ++i)
                weights[i] = 1.0 / Points[i].distance;
            double weightSum = weights.Sum();
            for (int i = 0; i < K; ++i)
                weights[i] /= weightSum;

            // Vote
            Dictionary<T, double> votes = new Dictionary<T, double>(Points.Select(p => p.label).Distinct().Select(p => new KeyValuePair<T, double>(p, 0)));
            for (int i = 0; i < K; ++i)
            {
                T classLabel = Points[i].label;
                votes[classLabel] += weights[i];
            }

            //Predict using ArgMax of votes
            T prediction = votes.First().Key;
            double maxVal = votes.First().Value;
            foreach (KeyValuePair<T, double> vote in votes)
            {
                if (vote.Value > maxVal)
                {
                    maxVal = vote.Value;
                    prediction = vote.Key;
                }
            }

            return prediction;
        }

        /// <summary>
        /// Test the error and accuracy of a set of labeled data.
        /// </summary>
        /// <param name="testData">The data to test the model against.</param>
        /// <returns>The average prediction error and the percent accuracy.</returns>
        public (double Error, double Accuracy) Test(LabelledData<T> testData)
        {
            return Test(testData.Features, testData.Labels);
        }

        /// <summary>
        /// Test the error and accuracy of a set of labeled data.
        /// </summary>
        /// <param name="features">The features (x-values) of the test data.</param>
        /// <param name="labels">The labels (y-values) of the test data.</param>
        /// <returns>The average prediction error and the percent accuracy.</returns>
        public (double Error, double Accuracy) Test(double[][] features, T[] labels)
        {
            throw new NotImplementedException();
        }
    }
}
