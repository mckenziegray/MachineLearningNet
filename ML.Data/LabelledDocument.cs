using System.Collections.Generic;

namespace ML.Data
{
    /// <summary>
    /// A labelled data set containing a list of words
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
