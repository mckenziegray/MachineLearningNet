using System;
using System.Collections.Generic;
using System.Linq;
using DotNetExtensions;
using ML.Data;

namespace ML.Classification
{
    public class DecisionTreeModel<T> : IClassificationModel<T>
    {
        #region Internal Classes
        /// <summary>
        /// A node within the tree (either a <see cref="Branch"/> or a <see cref="Leaf"/>.
        /// </summary>
        protected abstract class Node
        {
            public int Depth { get; protected set; }
            public LabelledData<T> Data { get; init; }

            public Node(LabelledData<T> data, int depth)
            {
                Data = data;
                Depth = depth;
            }

            /// <summary>
            /// Creates a new <see cref="Branch"/> or <see cref="Leaf"/>, depending on the given data and depth.
            /// </summary>
            /// <param name="data">The data to use for this node.</param>
            /// <param name="depth">The depth of this node within the tree.</param>
            /// <param name="maxDepth">The depth at which to stop branching.</param>
            /// <returns>
            /// Returns a <see cref="Branch"/> if:
            ///     1. The max depth has not been reached; and 
            ///     2. Splitting the data can still result in information gain.
            /// Otherwise, returns a <see cref="Leaf"/>.
            /// </returns>
            public static Node Sprout(LabelledData<T> data, int depth, int maxDepth)
            {
                // If we've reached the max depth, stop branching
                if (depth == maxDepth)
                    return new Leaf(data, depth);

                int splitColIdx = -1;
                double threshold = double.NaN;
                double maxGain = 0;

                // Find the best column to split on
                for (int i = 0; i < data.Features.ColumnCount; ++i)
                {
                    IOrderedEnumerable<(double Value, T Label)> curColumn = data.Rows
                        .Select(r => (r.DataRow[i], r.Label))
                        .OrderBy(c => c.Item1);

                    // Determine the optimal threshold for this column
                    double colThreshold = double.NaN;
                    double colMaxGain = 0;

                    foreach ((double value, T label) in curColumn)
                    {
                        // Calculate the info gain for the current column split on the current value
                        double infoGain = InfoGain(curColumn, value, data.Features.ColumnCount, data.AllLabels);

                        if (infoGain > colMaxGain)
                        {
                            colMaxGain = infoGain;
                            colThreshold = value;
                        }
                    }

                    // Record the column and threshold with the highest information gain
                    if (colMaxGain > maxGain)
                    {
                        maxGain = colMaxGain;
                        threshold = colThreshold;
                        splitColIdx = i;
                    }
                }

                // If there's no information gain, stop branching
                if (maxGain == 0)
                    return new Leaf(data, depth);

                return new Branch(data, depth, maxDepth, splitColIdx, threshold);
            }

            /// <summary>
            /// Calculate the total information gain from the given column and threshold.
            /// </summary>
            /// <param name="column"></param>
            /// <param name="threshold">The value on which to split the column.</param>
            /// <param name="numColumns">The total number of feature columns in the data set.</param>
            /// <returns></returns>
            protected static double InfoGain(IEnumerable<(double Value, T Label)> column, double threshold, int numColumns, IEnumerable<T> allLabels)
            {
                IEnumerable<(double Value, T Label)> lesserSide;
                IEnumerable<(double Value, T Label)> greaterSide;

                (lesserSide, greaterSide) = column.Split(threshold, c => c.Value);

                double pLeft = (double)lesserSide.Count() / numColumns; // Percent of values in the column less than or equal to the threshold
                double pRight = (double)greaterSide.Count() / numColumns; // Percent of values in the column greater than the threshold

                return Entropy(column, allLabels) - ((pLeft * Entropy(lesserSide, allLabels)) + (pRight * Entropy(greaterSide, allLabels)));
            }

            /// <summary>
            /// Calculate the amount of entropy for the data column. 
            /// Entropy is a measure of the amount of "uncertainty" within a data column. 
            /// </summary>
            /// <param name="column">A list of values in the column and their respective labels.</param>
            /// <returns>The entropy value for the data column.</returns>
            protected static double Entropy(IEnumerable<(double Value, T Label)> column, IEnumerable<T> allLabels)
            {
                if (!column.Any())
                    return 0;

                #region Calculate the probability of a random item in this column having a given label
                Dictionary<T, double> probDist = allLabels.ToDictionary(l => l, l => 0.0);

                foreach ((double val, T label) in column)
                {
                    if (!probDist.ContainsKey(label))
                        throw new ArgumentException($"{nameof(allLabels)} does not contain label {label}.");

                    ++probDist[label];
                }

                int colLen = column.Count();
                foreach (T label in probDist.Keys)
                {
                    probDist[label] /= colLen;
                }
                #endregion

                double entropy = probDist.Sum(p => -(p.Value * Math.Log2(p.Value)));

                return entropy;
            }
        }

        protected class Branch : Node
        {
            public int SplitColumnIndex { get; init; }
            public double SplitThreshold { get; init; }
            public Node LeftChild { get; protected set; }
            public Node RightChild { get; protected set; }

            public Branch(LabelledData<T> data, int depth, int maxDepth, int splitColIdx, double splitThreshold)
                : base(data, depth)
            {
                SplitColumnIndex = splitColIdx;
                SplitThreshold = splitThreshold;

                List<double[]> leftFeatures = new();
                List<double[]> rightFeatures = new();
                List<T> leftLabels = new();
                List<T> rightLabels = new();

                for (int i = 0; i < data.Labels.Length; ++i)
                {
                    if (data.Features[i][SplitColumnIndex] > SplitThreshold)
                    {
                        rightFeatures.Add(data.Features[i]);
                        rightLabels.Add(data.Labels[i]);
                    }
                    else
                    {
                        leftFeatures.Add(data.Features[i]);
                        leftLabels.Add(data.Labels[i]);
                    }
                }

                LeftChild = Sprout(new LabelledData<T>(leftFeatures.ToArray(), leftLabels.ToArray()), depth + 1, maxDepth);
                RightChild = Sprout(new LabelledData<T>(rightFeatures.ToArray(), rightLabels.ToArray()), depth + 1, maxDepth);
            }

            public Node Next(double[] features)
            {
                return features[SplitColumnIndex] > SplitThreshold ? RightChild : LeftChild;
            }
        }

        protected class Leaf : Node
        {
            public T Value { get; init; }
            public double Confidence { get; init; }

            public Leaf(LabelledData<T> data, int depth)
                : base(data, depth)
            {
                if (Data?.Labels?.Any() != true)
                    throw new NotSupportedException("Can't predict class without data.");

                Dictionary<T, int> classCounts = Data.AllLabels.ToDictionary(l => l, _ => 0);
                foreach (T label in Data.Labels)
                    ++classCounts[label];

                int largestCount = 0;
                T predictedClass = Data.Labels.First();

                foreach (KeyValuePair<T, int> classCount in classCounts)
                {
                    if (classCount.Value > largestCount)
                    {
                        largestCount = classCount.Value;
                        predictedClass = classCount.Key;
                    }
                }

                Value = predictedClass;
                Confidence = (double)largestCount / (double)Data.Rows.Count;
            }
        }
        #endregion

        protected Node Root { get; set; }

        public DecisionTreeModel(LabelledData<T> trainingData, int maxDepth)
        {
            Root = Node.Sprout(trainingData, 0, maxDepth);
        }

        /// <summary>
        /// Retrieve the <see cref="Leaf"/> that results from traversing the tree using the given feature set.
        /// </summary>
        /// <param name="features">The data row for which to predict a class.</param>
        /// <returns>
        /// A <see cref="Leaf"/> containing the predicted class as well as the level of confidence in that prediction.
        /// </returns>
        protected Leaf Predict(double[] features)
        {
            Node curNode = Root;
            while (curNode is Branch branch)
                curNode = branch.Next(features);

            return (Leaf)curNode;
        }

        public T Classify(double[] features)
        {
            return Predict(features).Value;
        }

        public T[] ClassifyAll(Matrix<double> features)
        {
            return features.Select(r => Classify(r)).ToArray();
        }

        public (double Error, double Accuracy) Test(LabelledData<T> data)
        {
            double errSum = 0;
            int accSum = 0;

            foreach ((double[] dataRow, T label) in data.Rows)
            {
                Leaf result = Predict(dataRow);

                double error = 1 - result.Confidence;

                errSum += error * error;

                if (result.Value.Equals(label))
                    accSum += 1;
            }

            return (errSum / data.Rows.Count, (double)accSum / data.Rows.Count);
        }
    }
}
