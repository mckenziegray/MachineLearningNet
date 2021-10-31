using DotNetExtensions;
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
                return distance.CompareTo(other.distance);
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
        {
            int numRows = trainingData.Features.RowCount;

            Points = new LabeledVector[numRows];
            for (int i = 0; i < numRows; i++)
            {
                Points[i].features = trainingData.Features[i];
                Points[i].label = trainingData.Labels[i];
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
            return Predict(features).Label;
        }

        /// <summary>
        /// Predict the classification of every feature vector in the given matrix.
        /// </summary>
        /// <param name="features">A matrix of feature vectors.</param>
        /// <returns>An array of classification labels.</returns>
        public T[] ClassifyAll(Matrix<double> features)
        {
            return features.Select(v => Classify(v)).ToArray();
        }

        /// <summary>
        /// Predict the label of a feature vector.
        /// </summary>
        /// <param name="features">The feature vector to classify.</param>
        /// <returns>A tuple containing the predicted label and a value representing the confidence in the selection (based on voting) as a percent.</returns>
        protected (T Label, double Confidence) Predict(double[] features)
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
            Dictionary<T, double> votes = new(Points.Select(p => p.label).Distinct().Select(p => new KeyValuePair<T, double>(p, 0)));
            for (int i = 0; i < K; ++i)
            {
                T classLabel = Points[i].label;
                votes[classLabel] += weights[i];
            }

            //Predict using ArgMax of votes
            T prediction = votes.First().Key;
            double maxVal = double.NegativeInfinity;
            foreach (KeyValuePair<T, double> vote in votes)
            {
                if (vote.Value > maxVal)
                {
                    maxVal = vote.Value;
                    prediction = vote.Key;
                }
            }

            return (prediction, votes.Where(v => v.Key.Equals(prediction)).Select(v => v.Value).Sum() / votes.Values.Sum());
        }

        /// <summary>
        /// Test the error and accuracy of a set of labeled data.
        /// </summary>
        /// <param name="data">The data to test the model against.</param>
        /// <returns>The average prediction error and the percent accuracy.</returns>
        public (double Error, double Accuracy) Test(LabelledData<T> data)
        {
            double errSum = 0;
            int accSum = 0;

            for (int i = 0; i < data.Features.RowCount; ++i)
            {
                (T prediction, double confidence) = Predict(data.Features[i]);

                double error = 1 - confidence;
                errSum += error * error;

                if (prediction.Equals(data.Labels[i]))
                    accSum += 1;
            }

            return (errSum / data.Features.RowCount, accSum * 1.0 / data.Features.RowCount);
        }
    }
}
