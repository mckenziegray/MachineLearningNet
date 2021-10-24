using ML.Data;

namespace ML
{
    public interface IModel<T>
    {
        /// <summary>
        /// Tests the model against the given data set.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>
        /// Error: The average sum of squared error against the data set.
        /// Accuracy: The accuracy of the model against the data set, expressed as a number between 0 and 1, where 1 is 100% accuracy.
        /// </returns>
        (double Error, double Accuracy) Test(LabelledData<T> data);
    }
}
