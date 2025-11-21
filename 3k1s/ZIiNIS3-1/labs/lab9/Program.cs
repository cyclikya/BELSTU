using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalMethods
{
    public class CharacterInfo
    {
        public string Character;
        public float Probability;
    }

    class Program
    {
        const int mantis = 4;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string text = "УгоренкоВиолеттаДинамические вероятности";
            Console.WriteLine("Исходное сообщение: " + text);

            // Динамические вероятности из самого сообщения
            var dynamicTable = BuildDynamicTable(text);
            PrintTable(dynamicTable, "Динамические вероятности");

            // Шеннона-Фано
            var sfCodes = ShannonFano(dynamicTable);
            Console.WriteLine("\nКоды Шеннона–Фано:");
            foreach (var c in sfCodes)
                Console.WriteLine($"{c.Key} : {c.Value}");

            // Хаффмана
            var huffCodes = Huffman(dynamicTable);
            Console.WriteLine("\nКоды Хаффмана:");
            foreach (var c in huffCodes)
                Console.WriteLine($"{c.Key} : {c.Value}");

            // Прямое кодирование
            string sfEncoded = Encode(text, sfCodes);
            string hfEncoded = Encode(text, huffCodes);

            Console.WriteLine("\nЗакодированное сообщение (Шеннона–Фано): " + sfEncoded);
            Console.WriteLine("Длина: " + sfEncoded.Length);

            Console.WriteLine("\nЗакодированное сообщение (Хаффмана): " + hfEncoded);
            Console.WriteLine("Длина: " + hfEncoded.Length);

            // Обратное преобразование
            Console.WriteLine("\nДекодирование SF: " + Decode(sfEncoded, sfCodes));
            Console.WriteLine("Декодирование Huffman: " + Decode(hfEncoded, huffCodes));

            // Эффективность относительно ASCII
            int asciiBits = text.Length * 8;
            Console.WriteLine("\n=== ЭФФЕКТИВНОСТЬ ===");
            Console.WriteLine($"ASCII: {asciiBits} бит");
            Console.WriteLine($"Шеннона–Фано: {sfEncoded.Length} бит (сжатие: {((double)asciiBits / sfEncoded.Length):F2} раз)");
            Console.WriteLine($"Хаффмана: {hfEncoded.Length} бит (сжатие: {((double)asciiBits / hfEncoded.Length):F2} раз)");

            Console.ReadKey();
        }

        // Табллица динамических вероятностей
        static Dictionary<string, float> BuildDynamicTable(string text)
        {
            var dict = new Dictionary<string, float>();

            int total = text.Length;

            foreach (char c in text)
            {
                string s = c.ToString();
                if (!dict.ContainsKey(s))
                    dict[s] = 0;

                dict[s]++;
            }

            foreach (var key in dict.Keys.ToList())
                dict[key] = (float)Math.Round(dict[key] / total, mantis);

            return dict.OrderByDescending(p => p.Value)
                       .ToDictionary(p => p.Key, p => p.Value);
        }

        static void PrintTable(Dictionary<string, float> table, string title)
        {
            Console.WriteLine($"\n=== {title} ===");
            foreach (var p in table)
                Console.WriteLine($"{p.Key} : {p.Value}");
        }

        // Shannon–Fano
        static Dictionary<string, string> ShannonFano(Dictionary<string, float> table)
        {
            var codes = table.ToDictionary(p => p.Key, p => "");
            ShannonSplit(0, table.Count, table, codes);
            return codes;
        }

        static void ShannonSplit(int start, int end, Dictionary<string, float> table, Dictionary<string, string> code)
        {
            if (end - start <= 1) return;

            float total = 0;
            for (int i = start; i < end; i++)
                total += table.ElementAt(i).Value;

            float partial = 0;
            int split = start;

            for (int i = start; i < end; i++)
            {
                partial += table.ElementAt(i).Value;
                if (partial >= total / 2)
                {
                    split = i + 1;
                    break;
                }
            }

            for (int i = start; i < split; i++)
                code[table.ElementAt(i).Key] += "1";

            for (int i = split; i < end; i++)
                code[table.ElementAt(i).Key] += "0";

            ShannonSplit(start, split, table, code);
            ShannonSplit(split, end, table, code);
        }

        // Huffman
        class Node
        {
            public string Symbol;
            public float P;
            public Node Left, Right;
        }

        static Dictionary<string, string> Huffman(Dictionary<string, float> table)
        {
            var nodes = table.Select(p => new Node { Symbol = p.Key, P = p.Value }).ToList();

            while (nodes.Count > 1)
            {
                nodes = nodes.OrderBy(n => n.P).ToList();

                var a = nodes[0];
                var b = nodes[1];

                var parent = new Node
                {
                    Symbol = a.Symbol + b.Symbol,
                    P = a.P + b.P,
                    Left = a,
                    Right = b
                };

                nodes.Remove(a);
                nodes.Remove(b);
                nodes.Add(parent);
            }

            var root = nodes[0];
            var codes = new Dictionary<string, string>();
            BuildHuffmanCodes(root, "", codes);
            return codes;
        }

        static void BuildHuffmanCodes(Node n, string prefix, Dictionary<string, string> codes)
        {
            if (n.Left == null && n.Right == null)
            {
                codes[n.Symbol] = prefix;
                return;
            }

            BuildHuffmanCodes(n.Left, prefix + "0", codes);
            BuildHuffmanCodes(n.Right, prefix + "1", codes);
        }

        // Прямое и обратное кодирование
        static string Encode(string text, Dictionary<string, string> code)
        {
            StringBuilder sb = new();
            foreach (char c in text)
                sb.Append(code[c.ToString()]);
            return sb.ToString();
        }

        static string Decode(string encoded, Dictionary<string, string> code)
        {
            string result = "";
            string current = "";

            var reverse = code.ToDictionary(p => p.Value, p => p.Key);

            foreach (char bit in encoded)
            {
                current += bit;
                if (reverse.ContainsKey(current))
                {
                    result += reverse[current];
                    current = "";
                }
            }

            return result;
        }
    }
}
