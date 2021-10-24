using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ML.Data;

namespace ML.NLP
{
    public class TFIDF<T> : IWordEmbeddingModel<T>
    {
        public LabelledData<T> Embed(LabelledCorpus<T> corpus)
        {
            // TODO: Implement!
            throw new NotImplementedException();
        } 

        protected static double TF(string word, Document document)
        {
            return document.Words.Count(w => w == word) / document.Words.Length;
        }

        protected static double IDF(string word, Corpus corpus)
        {
            return Math.Log10(corpus.Documents.Count / corpus.Documents.Count(d => d.Words.Contains(word)));
        }
    }
}
