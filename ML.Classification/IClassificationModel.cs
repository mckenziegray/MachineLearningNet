namespace ML.Classification
{
    public interface IClassificationModel<T> : IModel<T>
    {
        T Classify(double[] features);
    }
}
