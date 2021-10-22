using System.Collections.Generic;
using System.Linq;

namespace ML.Data
{
    /// <summary>
    /// An unlabelled data set containing a list of words
    /// </summary>
    public class Document
    {
        public string[] Words { get; protected set; }

        public Document(IEnumerable<string> words)
        {
            Words = words.ToArray();
        }
    }
}
