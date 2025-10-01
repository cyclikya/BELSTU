using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using static laba2.Entropy;

namespace laba2
{
    public static class Entropy
    {
        public static double ComputeDatsky(float errorProbability = 0)
        {
            var alphabet = "abcdefghijklmnopqrstuvwxyzæøå";
            var path = "datsky.txt";

            return ComputeEntropy(alphabet, path, errorProbability);
        }
        public static double ComputeKazach(float errorProbability = 0)
        {
            var path = "kazach.txt";
            var alphabet = "аәбвгғдеёжзийкқлмнңоөпрстуұүфхһцчшщъыіьэюя";

            return ComputeEntropy(alphabet, path, errorProbability);
        }
        public static double ComputeBinary(float errorProbability = 0)
        {
            var path = "binary.txt";
            var alphabet = "01";

            return ComputeEntropy(alphabet, path, errorProbability);
        }

        public static double ComputeEntropy(string alphabet, string path, float errorProbability = 0)
        {
            Dictionary<char, int> numberOfOccurrences = new Dictionary<char, int>();
            foreach (var ch in alphabet)
                numberOfOccurrences.Add(ch, 0);

            using (StreamReader sr = new StreamReader(path))
            {
                string text = sr.ReadToEnd();
                text = text.ToLower();
                foreach (var ch in text.Select((value, i) => new { i, value }))
                {
                    if (alphabet.Contains(ch.value))
                        numberOfOccurrences[ch.value]++;
                }

                double answer = 0;
                foreach (var ch in alphabet)
                {
                    if (numberOfOccurrences[ch] != 0)
                    {
                        double p = (double)numberOfOccurrences[ch] / (double)text.Length * (1 - errorProbability);
                        answer += p * Math.Log2(p);
                    }
                }

                return -answer;
            }
        }
    }
}

