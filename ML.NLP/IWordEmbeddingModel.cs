using ML.Data;

namespace ML.NLP
{
    public interface IWordEmbeddingModel<T>
    {
        LabelledData<T> Embed(LabelledCorpus<T> corpus);
    }
}
