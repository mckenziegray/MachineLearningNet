using DotNetExtensions;
using ML.Data;

namespace ML
{
    public interface IModel<T>
    {
        (double Error, double Accuracy) Test(LabelledData<T> testData);
        (double Error, double Accuracy) Test(Matrix<double> features, T[] labels);
    }
}
