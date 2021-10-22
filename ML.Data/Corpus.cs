using System.Collections.Generic;
using System.Linq;

namespace ML.Data
{
    /// <summary>
    /// A data set containing a list of unlabelled "documents", which each contain a list of words
    /// </summary>
    public class Corpus
    {
        public List<Document> Documents { get; protected set; }

        public Corpus()
        {
            Documents = new List<Document>();
        }

        public Corpus(IEnumerable<Document> documents)
        {
            Documents = documents.ToList();
        }

        public void AddDocument(Document document)
        {
            Documents.Add(document);
        }
    }
}
