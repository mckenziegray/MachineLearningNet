using System.Collections.Generic;

namespace ML.Data
{
    public class LabelledCorpus<T>
    {
        public List<LabelledDocument<T>> Documents { get; protected set; } = new List<LabelledDocument<T>>();

        public void AddDocument(LabelledDocument<T> document)
        {
            Documents.Add(document);
        }
    }
}
