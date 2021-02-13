using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ML.Data
{
    public static class DataLoader
    {
        public static Data LoadUnlabelledData(string fileName, char delimiter, int skipLines = 0)
        {
            List<double[]> data = new List<double[]>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                for (int i = 0; i < skipLines; ++i)
                    reader.ReadLine();

                while (!reader.EndOfStream)
                    data.Add(reader.ReadLine().Split(delimiter).Select(d => Double.Parse(d)).ToArray());
            }

            return new Data(data.ToArray());
        }

        public static LabelledData<T> LoadLabelledData<T>(string fileName, char delimiter, int labelColumnIndex, int skipLines = 0)
        {
            List<double[]> features = new List<double[]>();
            List<T> labels = new List<T>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                for (int i = 0; i < skipLines; ++i)
                    reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string[] dataRow = reader.ReadLine().Split(delimiter);
                    int j = 0;
                    double[] x = new double[dataRow.Length - 1];
                    for (int i = 0; i < dataRow.Length; i++)
                    {
                        if (i == labelColumnIndex)
                            labels.Add((T)Convert.ChangeType(dataRow[i], typeof(T)));
                        else
                        {
                            x[j] = Double.Parse(dataRow[i]);
                            ++j;
                        }
                    }

                    features.Add(x);
                }

                return new LabelledData<T>(features.ToArray(), labels.ToArray());
            }
        }

        public static Document LoadUnlabelledDocument(string fileName)
        {
            return new Document(GetAndCleanWordsFromFile(fileName));
        }

        public static LabelledDocument<T> LoadLabelledDocument<T>(string fileName, T label)
        {
            return new LabelledDocument<T>(GetAndCleanWordsFromFile(fileName), label);
        }
        private static IEnumerable<string> GetAndCleanWordsFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd().ToLower().Where(c => !Char.IsPunctuation(c)).ToString().Split(null);
            }
        }
    }
}
