using ML.Data;
using System;
using System.Linq;

namespace ML.Classification
{
    public class LogisticRegressionModel : IBinaryClassificationModel
    {
        public double[] Weights { get; protected set; }

        public LogisticRegressionModel(LabelledData<bool> trainingData, double learningRate, int numEpochs)
            : this(trainingData.Features, trainingData.Labels, learningRate, numEpochs)
        {}

        public LogisticRegressionModel(double[][] features, bool[] labels, double learningRate, int numEpochs)
        {
            if (features.Length != labels.Length)
                throw new ArgumentException($"The number of data rows is not equal to the number of labels. Rows: {features.Length}\n Labels: {labels.Length}");

            Train(features, labels.Select(l => l ? 1 : 0).ToArray(), learningRate, numEpochs);
        }

        /// <summary>
        /// Train the model.
        /// </summary>
        /// <param name="features">Data used for classification; x-values.</param>
        /// <param name="labels">Labels indicating the correct classification for each feature vector; y-values.</param>
        /// <param name="learningRate">A Constant modifier that affects the rate at which the weights update.</param>
        /// <param name="numEpochs">Number of training iterations or "epochs".</param>
        protected void Train(double[][] features, int[] labels, double learningRate, int numEpochs)
        {
            int numItems = features.Length;
            int numFeatures = Utils.GetNumColumns(features);
            Weights = new double[numFeatures + 1];
            Random random = new Random();

            for (int i = 0; i < Weights.Length; ++i)
            {
                // Initialize the weights to a random value from -0.01 to 0.01
                Weights[i] = (random.NextDouble() - 0.5) / 50;
            }

            int[] indexes = new int[numItems];
            for (int i = 0; i < indexes.Length; ++i)
                indexes[i] = i;

            for (int epoch = 0; epoch < numEpochs; ++epoch)
            {
                // Randomize the order in which to check features
                indexes = indexes.OrderBy(i => random.Next()).ToArray();

                for (int i = 0; i < indexes.Length; ++i)
                {
                    int index = indexes[i];
                    double predictionValue = Predict(features[index]);

                    for (int j = 0; j < numFeatures; ++j)
                        Weights[j] = learningRate * features[index][j] * (labels[index] - predictionValue) * predictionValue * (1 - predictionValue);
                    Weights[numFeatures] = learningRate * (labels[index] - predictionValue) * predictionValue * (1 - predictionValue);
                }
            }
        }

        /// <summary>
        /// Test the error and accuracy of a set of labeled data.
        /// </summary>
        /// <param name="testData">The data to test the model against.</param>
        /// <returns>The average prediction error and the percent accuracy.</returns>
        public (double Error, double Accuracy) Test(LabelledData<bool> testData)
        {
            return Test(testData.Features, testData.Labels);
        }

        /// <summary>
        /// Test the error and accuracy of a set of labeled data.
        /// </summary>
        /// <param name="features">The features (x-values) of the test data.</param>
        /// <param name="labels">The labels (y-values) of the test data.</param>
        /// <returns>The average prediction error and the percent accuracy.</returns>
        public (double Error, double Accuracy) Test(double[][] features, bool[] labels)
        {
            double errSum = 0;
            int accSum = 0;

            for (int i = 0; i < features.Length; ++i)
            {
                double predictionValue = Predict(features[i]);

                double error = predictionValue - (labels[i] ? 1 : 0);
                errSum += error * error;

                if (Classify(predictionValue))
                    accSum += 1;
            }

            return (errSum / features.Length, (accSum * 1.0) / features.Length);
        }

        /// <summary>
        /// Predict the label of a feature vector.
        /// </summary>
        /// <param name="features">The vector of features to classify.</param>
        /// <returns>The predicted label.</returns>
        public bool Classify(double[] features)
        {
            return Classify(Predict(features));
        }

        /// <summary>
        /// Convert a numeric prediction value to a binary classification label.
        /// </summary>
        /// <param name="predictionValue">The value to convert.</param>
        /// <returns>The converted label.</returns>
        protected bool Classify(double predictionValue)
        {
            return predictionValue > 0.5;
        }

        /// <summary>
        /// Calculate the predicted y-value for a feature vector.
        /// </summary>
        /// <param name="features">The feature vector (x-values).</param>
        /// <returns>A number between 0 and 1 representing the predicted value.</returns>
        private double Predict(double[] features)
        {
            double z = 0;
            for (int i = 0; i < features.Length; ++i)
                z += features[i] * Weights[i];
            z += Weights.Last(); // Bias

            // Logarithmic sigmoid
            if (z < -20.0)
                return 0.0;
            else if (z > 20.0)
                return 1.0;
            return 1.0 / (1.0 + Math.Exp(-z));
        }
    }
}
