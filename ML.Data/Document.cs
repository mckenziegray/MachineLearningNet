using System.Collections.Generic;
using System.Linq;

namespace ML.Data
{
    public class Document
    {
        public List<string> Words { get; protected set; }

        public Document(IEnumerable<string> words)
        {
            Words = words.ToList();
        }
    }
}
