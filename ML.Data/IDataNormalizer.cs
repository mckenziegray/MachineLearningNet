using DotNetExtensions;

namespace ML.Data
{
    /// <summary>
    /// Interface for a system which can normalize a set of data
    /// </summary>
    public interface IDataNormalizer
    {
        /// <summary>
        /// Creates a normalized data set from the given <see cref="Data"/>.
        /// </summary>
        /// <param name="data">The data set to normalize.</param>
        /// <returns>A new <see cref="Data"/> with the data from <paramref name="data"/> normalized according to the implementation.</returns>
        Data Normalize(Data data);

        /// <summary>
        /// Creates a normalized data set from the given <see cref="Matrix{T}"/>.
        /// </summary>
        /// <param name="data">The data set to normalize.</param>
        /// <returns>A new <see cref="Matrix{T}"/> with the data from <paramref name="data"/> normalized according to the implementation.</returns>
        Matrix<double> Normalize(Matrix<double> data);
    }
}
