using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationSystemsAnalysis
{
    public class Program
    {
        private static readonly string base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private static readonly string datskyAlphabet = "abcdefghijklmnopqrstuvwxyzæøå,.!?";
        private static readonly char paddingChar = '=';

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("------------1");
            TestBase64Encoding();
            Console.WriteLine();

            Console.WriteLine("\n------------2");
            AnalyzeEntropyCorrectly();
            Console.WriteLine();

            Console.WriteLine("\n------------3");
            TestXOROperationsCorrectly();
        }

        public static void TestBase64Encoding()
        {
            string testText = "Sveikas, pasauli!";

            string myBase64 = EncodeToBase64Correct(testText);

            Console.WriteLine($"Датская фраза: {testText}");
            Console.WriteLine($"Мой Base64:      {myBase64}");

            byte[] bytes = Encoding.UTF8.GetBytes(testText);
            string netBase64 = Convert.ToBase64String(bytes);
            Console.WriteLine($".NET Base64:    {netBase64}");
            Console.WriteLine($"Совпадение: {myBase64 == netBase64}");
        }

        public static string EncodeToBase64Correct(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < bytes.Length; i += 3)
            {
                int byteCount = Math.Min(3, bytes.Length - i);

                int triple = 0;
                for (int j = 0; j < byteCount; j++)
                {
                    triple |= bytes[i + j] << (16 - j * 8);
                }

                result.Append(base64Alphabet[(triple >> 18) & 0x3F]);
                result.Append(base64Alphabet[(triple >> 12) & 0x3F]);

                if (byteCount > 1)
                {
                    result.Append(base64Alphabet[(triple >> 6) & 0x3F]);
                }
                else
                {
                    result.Append(paddingChar);
                }

                if (byteCount > 2)
                {
                    result.Append(base64Alphabet[triple & 0x3F]);
                }
                else
                {
                    result.Append(paddingChar);
                }
            }

            return result.ToString();
        }

        public static void AnalyzeEntropyCorrectly()
        {
            string text = "Sveikas, pasauli!";
            string base64Text = EncodeToBase64Correct(text);

            double textHartley = Math.Log(datskyAlphabet.Length, 2);
            double base64Hartley = Math.Log(base64Alphabet.Length + 1, 2);

            double textShannon = CalculateShannonEntropy(datskyAlphabet, text.ToLower());
            double base64Shannon = CalculateShannonEntropy(base64Alphabet + paddingChar, base64Text);

            double textRedundancy = (textHartley - textShannon) / textHartley;
            double base64Redundancy = (base64Hartley - base64Shannon) / base64Hartley;

            Console.WriteLine("-----Датский:");
            Console.WriteLine($"Кол-во уникальных символов: {GetUniqueCharacters(text.ToLower()).Count}");
            Console.WriteLine($"Энтропия(Хартли): {textHartley:F4} бит/символ");
            Console.WriteLine($"Энтропия(Шеннона): {textShannon:F4} бит/символ");
            Console.WriteLine($"Избыточность: {textRedundancy:P2}");
            Console.WriteLine();

            Console.WriteLine("-----Base64:");
            Console.WriteLine($"Кол-во уникальных символов: {GetUniqueCharacters(base64Text).Count}");
            Console.WriteLine($"Энтропия(Хартли): {base64Hartley:F4} бит/символ");
            Console.WriteLine($"Энтропия(Шеннона): {base64Shannon:F4} бит/символ");
            Console.WriteLine($"Избыточность: {base64Redundancy:P2}");
        }

        public static void TestXOROperationsCorrectly()
        {
            string a = "Ugorenko";
            string b = "Violetta";

            Console.WriteLine($"a='{a}'");
            Console.WriteLine($"b='{b}'");

            byte[] asciiA = StringToASCII(a);
            byte[] asciiB = StringToASCII(b);

            Console.WriteLine("\na в двоичном виде:");
            Console.WriteLine(BytesToBinaryString(asciiA));

            Console.WriteLine("\nb в двоичном виде:");
            Console.WriteLine(BytesToBinaryString(asciiB));

            byte[] xorAB = XORBuffers(asciiA, asciiB);

            Console.WriteLine("\na XOR b:");
            Console.WriteLine(BytesToBinaryString(xorAB));

            byte[] xorABB = XORBuffers(xorAB, asciiB);
            string result = ASCIIToString(xorABB);

            Console.WriteLine($"\na XOR b XOR b:'{result}'");
            Console.WriteLine($"\nПроверка: {result == a}");
        }

        public static byte[] StringToASCII(string text)
        {
            byte[] result = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                result[i] = (byte)text[i];
            }
            return result;
        }

        public static string ASCIIToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }
            return new string(chars);
        }

        public static string BytesToBinaryString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                sb.Append(' ');
            }
            return sb.ToString().Trim();
        }

        public static double CalculateShannonEntropy(string alphabet, string text)
        {
            var counts = new Dictionary<char, int>();
            foreach (var ch in alphabet) counts[ch] = 0;

            var filtered = text.Where(c => alphabet.Contains(c)).ToArray();
            int n = filtered.Length;
            if (n == 0) return 0.0;

            foreach (var c in filtered) counts[c]++;

            double H = 0.0;
            foreach (var ch in alphabet)
            {
                int occ = counts[ch];
                if (occ > 0)
                {
                    double p = (double)occ / n;
                    H += p * Math.Log2(p);
                }
            }

            return -H;
        }

        private static HashSet<char> GetUniqueCharacters(string text, string alphabet = null)
        {
            if (alphabet == null)
                return new HashSet<char>(text);
            return new HashSet<char>(text.Where(c => alphabet.Contains(c)));
        }

        private static byte[] XORBuffers(byte[] a, byte[] b)
        {
            int length = Math.Max(a.Length, b.Length);
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte byteA = i < a.Length ? a[i] : (byte)0;
                byte byteB = i < b.Length ? b[i] : (byte)0;
                result[i] = (byte)(byteA ^ byteB);
            }

            return result;
        }
    }
}