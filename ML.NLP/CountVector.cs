using ML.Data;
using System.Collections.Generic;
using System.Linq;

namespace ML.NLP
{
    public class CountVector<T> : IWordEmbeddingModel<T>
    {
        protected List<string> Vocabulary { get; set; }
        public LabelledData<T> CountMatrix { get; protected set; }

        public LabelledData<T> Embed(LabelledCorpus<T> corpus)
        {
            List<int[]> counts = new List<int[]>();
            Vocabulary = corpus.Documents.SelectMany(d => d.Words).Distinct().ToList();

            for (int i = 0; i < corpus.Documents.Count; ++i)
            {
                counts.Add(new int[Vocabulary.Count]);
                foreach (string word in corpus.Documents[i].Words)
                {
                    if (Vocabulary.Contains(word))
                        ++counts[i][Vocabulary.IndexOf(word)];
                }
            }

            CountMatrix = new LabelledData<T>(counts.Select(r => r.Select(c => (double)c).ToArray()).ToArray(), corpus.Documents.Select(d => d.Label).ToArray());
            return CountMatrix;
        }
    }
}
