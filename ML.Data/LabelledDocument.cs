using System.Collections.Generic;

namespace ML.Data
{
    public class LabelledDocument<T> : Document
    {
        public T Label { get; protected set; }

        public LabelledDocument(IEnumerable<string> words, T label)
            : base(words)
        {
            Label = label;
        }
    }
}
