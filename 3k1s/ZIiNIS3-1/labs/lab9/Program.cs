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
        const int mantisLength = 4;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // === ИСХОДНОЕ СООБЩЕНИЕ ===
            string text = "УгоренкоВиолеттаРомановна";

            Console.WriteLine("Исходный текст:");
            Console.WriteLine(text);

            // === ВЫБОР РЕЖИМА ===
            Console.WriteLine("\nВыберите режим сортировки символов:");
            Console.WriteLine("1 — использовать данные ЛР2 (статические вероятности)");
            Console.WriteLine("2 — динамически по сообщению");
            Console.Write("Ваш выбор: ");
            int mode = int.Parse(Console.ReadLine());

            Dictionary<string, float> valuePairs;

            if (mode == 1)
                valuePairs = GetStaticProbabilities_LR2();
            else
            {
                int _;
                var charsInfo = CalculateProbability(text, out _);
                valuePairs = charsInfo.OrderByDescending(p => p.Probability)
                                      .ToDictionary(p => p.Character, p => p.Probability);
            }

            Console.WriteLine("\nТаблица символов и вероятностей:");
            foreach (var item in valuePairs)
                Console.WriteLine($"{item.Key} : {item.Value}");

            // === ШЕННОН–ФАНО ===
            Console.WriteLine("\n=== Коды Шеннона–Фано ===");
            var codesSF = ShannonFano(valuePairs);

            foreach (var c in codesSF)
                Console.WriteLine($"{c.Key} : {c.Value}");

            // === ПРЯМОЕ КОДИРОВАНИЕ ===
            string encodedSF = Encode(text, codesSF);
            Console.WriteLine("\nЗашифрованное сообщение (Шеннон–Фано):");
            Console.WriteLine(encodedSF);

            // === ОБРАТНОЕ КОДИРОВАНИЕ ===
            string decodedSF = Decode(encodedSF, codesSF);
            Console.WriteLine("\nДекодированное сообщение:");
            Console.WriteLine(decodedSF);

            // === ЭФФЕКТИВНОСТЬ ===
            AnalyzeEfficiency(text, encodedSF);

            // === ХАФФМАН ===
            Console.WriteLine("\n=== Коды Хаффмана ===");
            var codesH = Huffman(valuePairs);

            foreach (var c in codesH)
                Console.WriteLine($"{c.Key} : {c.Value}");

            string encodedH = Encode(text, codesH);
            Console.WriteLine("\nЗашифрованное сообщение (Хаффман):");
            Console.WriteLine(encodedH);

            Console.WriteLine("\nДекодированное сообщение:");
            Console.WriteLine(Decode(encodedH, codesH));

            AnalyzeEfficiency(text, encodedH);

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        // =============== ЛР2 — СТАТИЧЕСКИЕ ВЕРОЯТНОСТИ ===================
        static Dictionary<string, float> GetStaticProbabilities_LR2()
        {
            // ДАННЫЕ ИЗ ЛР2 — ТЫ МОЖЕШЬ ИХ ДОБАВИТЬ САМ, Я СТАВЛЮ ШАБЛОН
            return new Dictionary<string, float>()
            {
                { "а", 0.08f }, { "н", 0.07f }, { "о", 0.06f },
                { "р", 0.06f }, { "и", 0.05f }, { "е", 0.05f },
                { "т", 0.04f }, { "у", 0.03f }, { "г", 0.02f },
                { "в", 0.02f }, { "л", 0.02f }
            }.OrderByDescending(p => p.Value)
             .ToDictionary(p => p.Key, p => p.Value);
        }

        // =============== ПРОБАБИЛИТИ ПО ТЕКСТУ ===================
        public static List<CharacterInfo> CalculateProbability(string text, out int total)
        {
            var result = new List<CharacterInfo>();
            total = 0;

            foreach (char c in text)
            {
                string s = c.ToString();
                if (!result.Any(x => x.Character == s))
                    result.Add(new CharacterInfo { Character = s });

                total++;
            }

            foreach (var item in result)
            {
                int count = text.Count(c => c == item.Character[0]);
                item.Probability = (float)Math.Round((double)count / total, mantisLength);
            }

            return result;
        }

        // =============== ШЕННОН–ФАНО ===================
        static Dictionary<string, string> ShannonFano(Dictionary<string, float> valuePairs)
        {
            var codes = valuePairs.ToDictionary(p => p.Key, p => "");
            BuildSF(0, valuePairs.Count, valuePairs, codes);
            return codes;
        }

        static void BuildSF(int start, int end, Dictionary<string, float> values, Dictionary<string, string> codes)
        {
            if (end - start <= 1)
                return;

            float total = 0;
            for (int i = start; i < end; i++)
                total += values.ElementAt(i).Value;

            float current = 0;
            int sep = start;

            for (int i = start; i < end; i++)
            {
                current += values.ElementAt(i).Value;
                if (current >= total / 2)
                {
                    sep = i + 1;
                    break;
                }
            }

            for (int i = start; i < sep; i++)
                codes[values.ElementAt(i).Key] += "0";

            for (int i = sep; i < end; i++)
                codes[values.ElementAt(i).Key] += "1";

            BuildSF(start, sep, values, codes);
            BuildSF(sep, end, values, codes);
        }

        // =============== ХАФФМАН ===================
        class Node
        {
            public string Symbol;
            public float P;
            public Node Left, Right;
        }

        static Dictionary<string, string> Huffman(Dictionary<string, float> values)
        {
            List<Node> nodes = values.Select(v => new Node { Symbol = v.Key, P = v.Value }).ToList();

            while (nodes.Count > 1)
            {
                var ordered = nodes.OrderBy(n => n.P).ToList();

                var left = ordered[0];
                var right = ordered[1];

                var parent = new Node
                {
                    Symbol = left.Symbol + right.Symbol,
                    P = left.P + right.P,
                    Left = left,
                    Right = right
                };

                nodes.Remove(left);
                nodes.Remove(right);
                nodes.Add(parent);
            }

            Dictionary<string, string> codes = new Dictionary<string, string>();
            BuildHuffman(nodes[0], "", codes);
            return codes;
        }

        static void BuildHuffman(Node node, string code, Dictionary<string, string> dict)
        {
            if (node.Left == null && node.Right == null)
            {
                dict[node.Symbol] = code;
                return;
            }

            BuildHuffman(node.Left, code + "0", dict);
            BuildHuffman(node.Right, code + "1", dict);
        }

        // =============== КОДИРОВАНИЕ ===================
        static string Encode(string text, Dictionary<string, string> codes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
                sb.Append(codes[c.ToString()]);
            return sb.ToString();
        }

        static string Decode(string encoded, Dictionary<string, string> codes)
        {
            Dictionary<string, string> reversed =
                codes.ToDictionary(p => p.Value, p => p.Key);

            string result = "";
            string buffer = "";

            foreach (char bit in encoded)
            {
                buffer += bit;
                if (reversed.ContainsKey(buffer))
                {
                    result += reversed[buffer];
                    buffer = "";
                }
            }

            return result;
        }

        // =============== ЭФФЕКТИВНОСТЬ ===================
        static void AnalyzeEfficiency(string text, string encoded)
        {
            int asciiBits = text.Length * 8;
            int codeBits = encoded.Length;

            double k = (double)asciiBits / codeBits;

            Console.WriteLine("\n=== Эффективность ===");
            Console.WriteLine($"Длина ASCII: {asciiBits} бит");
            Console.WriteLine($"Длина кодированного: {codeBits} бит");
            Console.WriteLine($"Коэффициент сжатия: {k:F4}");
            Console.WriteLine($"Выигрыш: {(k - 1) * 100:F2}%");
        }
    }
}
