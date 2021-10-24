using DotNetExtensions;

namespace ML.Classification
{
    /// <summary>
    /// Interface for an ML model for classifying data.
    /// </summary>
    /// <typeparam name="T">The data type of the class labels.</typeparam>
    public interface IClassificationModel<T> : IModel<T>
    {
        /// <summary>
        /// Predict the label of a feature vector.
        /// </summary>
        /// <param name="features">The vector of features to classify.</param>
        /// <returns>The predicted label.</returns>
        T Classify(double[] features);

        /// <summary>
        /// Predict the label of each feature vector.
        /// </summary>
        /// <param name="features">The set of feature vectors to classify.</param>
        /// <returns>
        /// An array containing the predicted labels.
        /// The index of each label corresponds to the index of the row in <paramref name="features"/>.
        /// </returns>
        T[] ClassifyAll(Matrix<double> features);
    }
}
