using DotNetExtensions;

namespace ML.Data
{
    public interface IDataNormalizer
    {
        Data Normalize(Data data);

        Data Normalize(Matrix<double> data);
    }
}
