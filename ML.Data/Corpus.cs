using System.Collections.Generic;

namespace ML.Data
{
    public class Corpus
    {
        public List<Document> Documents { get; protected set; } = new List<Document>();

        public void AddDocument(Document document)
        {
            Documents.Add(document);
        }
    }
}
