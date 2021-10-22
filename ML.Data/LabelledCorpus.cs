using System.Collections.Generic;

namespace ML.Data
{
    /// <summary>
    /// A data set containing a list of labelled "documents", which each contain a list of words
    /// </summary>
    public class LabelledCorpus<T>
    {
        public List<LabelledDocument<T>> Documents { get; protected set; } = new List<LabelledDocument<T>>();

        public void AddDocument(LabelledDocument<T> document)
        {
            Documents.Add(document);
        }
    }
}
