using ML.Data;

namespace ML
{
    public interface IModel<T>
    {
        (double Error, double Accuracy) Test(LabelledData<T> testData);
        (double Error, double Accuracy) Test(double[][] features, T[] labels);
    }
}
