using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ML.Data
{
    /// <summary>
    /// Helper class for extracting data from a CSV file
    /// </summary>
    public static class DataLoader
    {
        /// <summary>
        /// Reads a CSV file and loads its data into a <see cref="Data"/> object.
        /// </summary>
        /// <param name="fileName">The full path and name of the CSV file to read.</param>
        /// <param name="delimiter">The character used to separate values in the CSV file. Defaults to a comma (',').</param>
        /// <param name="skipLines">The number of lines in the file to skip before reading data. Defaults to 0.</param>
        /// <returns>A <see cref="Data"/> object containing the unlabelled data from the file.</returns>
        public static Data LoadUnlabelledData(string fileName, char delimiter = ',', int skipLines = 0)
        {
            List<double[]> data = new();

            using (StreamReader reader = new(fileName))
            {
                for (int i = 0; i < skipLines; ++i)
                    _ = reader.ReadLine();

                while (!reader.EndOfStream)
                    data.Add(reader.ReadLine().Split(delimiter).Select(d => double.Parse(d)).ToArray());
            }

            return new Data(data.ToArray());
        }

        /// <summary>
        /// Reads a CSV file and loads its data into a <see cref="LabelledData{T}"/> object.
        /// </summary>
        /// <typeparam name="T">The data type of the labels.</typeparam>
        /// <param name="fileName">The full path and name of the CSV file to read.</param>
        /// <param name="delimiter">The character used to separate values in the CSV file. Defaults to a comma (',').</param>
        /// <param name="labelColumnIndex">The column index for the data column containing the labels.</param>
        /// <param name="skipLines">The number of lines in the file to skip before reading data. Defaults to 0.</param>
        /// <returns>A <see cref="Data"/> object containing the labelled data from the file.</returns>
        public static LabelledData<T> LoadLabelledData<T>(string fileName, char delimiter = ',', int labelColumnIndex = 0, int skipLines = 0)
        {
            List<double[]> features = new();
            List<T> labels = new();

            using (StreamReader reader = new(fileName))
            {
                for (int i = 0; i < skipLines; ++i)
                    _ = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string[] dataRow = reader.ReadLine().Split(delimiter);
                    int j = 0;
                    double[] x = new double[dataRow.Length - 1];
                    for (int i = 0; i < dataRow.Length; i++)
                    {
                        if (i == labelColumnIndex)
                        {
                            labels.Add((T)Convert.ChangeType(dataRow[i], typeof(T)));
                        }
                        else
                        {
                            x[j++] = double.Parse(dataRow[i]);
                        }
                    }

                    features.Add(x);
                }
            }

            return new LabelledData<T>(features.ToArray(), labels.ToArray());
        }

        /// <summary>
        /// Reads a text file and loads its content into a <see cref="Document"/>.
        /// </summary>
        /// <param name="fileName">The full path and name of the file to read.</param>
        /// <param name="lowercase">
        /// Whether all of the words in the document should be converted to lowercase. 
        /// Retains original casing if false. Defaults to true.
        /// </param>
        /// <param name="includePunctuation">
        /// Whether punctuation should be retained from the text. 
        /// Removes punctuation if false. Defaults to false.
        /// </param>
        /// <param name="separators">
        /// Any number of characters to use when splitting the file's text into words.
        /// If no separators are provided, words will be split on whitespace.
        /// </param>
        /// <returns>A <see cref="Document"/> containing all of the words in the file.</returns>
        public static Document LoadUnlabelledDocument(string fileName, bool lowercase = true, bool includePunctuation = false, params char[] separators)
        {
            return new Document(GetWordsFromFile(fileName, lowercase, includePunctuation, separators));
        }

        /// <summary>
        /// Reads a text file and loads its content into a <see cref="LabelledDocument{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">The full path and name of the file to read.</param>
        /// <param name="label">The label for this document.</param>
        /// <param name="lowercase">
        /// Whether all of the words in the document should be converted to lowercase. 
        /// Retains original casing if false. Defaults to true.
        /// </param>
        /// <param name="includePunctuation">
        /// Whether punctuation should be retained from the text. 
        /// Removes punctuation if false. Defaults to false.
        /// </param>
        /// <param name="separators">
        /// Any number of characters to use when splitting the file's text into words.
        /// If no separators are provided, words will be split on whitespace.
        /// </param>
        /// <returns>A <see cref="LabelledDocument{T}"/> with label <see cref="label"/> and containing all of the words in the file.</returns>
        public static LabelledDocument<T> LoadLabelledDocument<T>(string fileName, T label, bool lowercase = true, bool includePunctuation = false, params char[] separators)
        {
            return new LabelledDocument<T>(GetWordsFromFile(fileName, lowercase, includePunctuation, separators), label);
        }

        private static IEnumerable<string> GetWordsFromFile(string fileName, bool lowercase = true, bool includePunctuation = false, params char[] separators)
        {
            IEnumerable<string> words;

            using (StreamReader reader = new(fileName))
            {
                words = reader.ReadToEnd().Split(separators);
            }

            if (lowercase)
                words = words.Select(w => w.ToLower());

            if (!includePunctuation)
                words = words.Select(w => w.Select(c => !char.IsPunctuation(c)).ToString()).Where(w => !string.IsNullOrEmpty(w));

            return words;
        }
    }
}
