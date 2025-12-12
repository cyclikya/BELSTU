using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab11
{
    public class Program
    {
        static void Main(string[] args)
        {
            string word = "достопримечательность";
            Run(word);

            Console.WriteLine();

            string word2 = "достопримечательностьсорокадневный";
            Run(word2);

            Console.ReadLine();
        }

        static void Run(string word)
        {
            Console.WriteLine($"=== Арифметическое сжатие слова '{word}' ===\n");

            int len = word.Length;
            Compressor c = new Compressor();
            c.Build(word);

            Console.WriteLine("Вероятности:");
            foreach (var n in c.Nodes)
                Console.WriteLine($"{n.Symbol}: {n.High - n.Low}");

            Console.WriteLine("\nИнтервалы:");
            foreach (var n in c.Nodes)
                Console.WriteLine($"{n.Symbol}  {FormatDecimal(n.Low)} - {FormatDecimal(n.High)}");

            Console.WriteLine();
            decimal code = c.Compress(word);

            Console.WriteLine($"\nВ качестве выходной дроби берется левая граница последнего диапазона");
            Console.WriteLine($"Результат: {code}\n");

            string restored = c.Decompress(code, len);
            Console.WriteLine($"\nРезультат: {restored}");
            Console.WriteLine($"Совпадение с исходным: {restored == word}");
        }

        // Форматирование десятичного числа для вывода (сокращенная версия)
        static string FormatDecimal(decimal value, int maxDigits = 15)
        {
            string str = value.ToString();

            // Если строка слишком длинная, обрезаем её
            if (str.Length > maxDigits + 2) // +2 для "0," или "0."
            {
                int decimalPoint = str.IndexOf('.');
                if (decimalPoint == -1) decimalPoint = str.IndexOf(',');

                if (decimalPoint != -1 && decimalPoint < str.Length - 1)
                {
                    // Оставляем целую часть и maxDigits знаков после запятой
                    int digitsToKeep = Math.Min(maxDigits, str.Length - decimalPoint - 1);
                    if (digitsToKeep > 0)
                    {
                        str = str.Substring(0, decimalPoint + digitsToKeep + 1);
                    }
                }
                else if (str.Length > maxDigits)
                {
                    str = str.Substring(0, maxDigits);
                }
            }

            return str;
        }
    }

    public class Compressor
    {
        public List<Node> Nodes { get; set; }
        public Dictionary<char, decimal> Frequencies { get; set; }
        public Node Range { get; set; }

        public void Build(string src)
        {
            Frequencies = new Dictionary<char, decimal>();
            decimal inc = 1m / src.Length;

            foreach (var ch in src)
            {
                if (!Frequencies.ContainsKey(ch))
                    Frequencies[ch] = 0;
                Frequencies[ch] += inc;
            }

            Frequencies = Frequencies
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            Nodes = new List<Node>();
            decimal low = 0m;

            foreach (var kv in Frequencies)
            {
                decimal high = low + kv.Value;
                Nodes.Add(new Node
                {
                    Symbol = kv.Key,
                    Low = low,
                    High = high
                });
                low = high;
            }
        }

        public decimal Compress(string src)
        {
            Range = new Node { Low = 0m, High = 1m };
            int step = 0;

            foreach (var ch in src)
            {
                step++;
                decimal oldLow = Range.Low;
                decimal oldHigh = Range.High;

                // Увеличиваем точность отображения с каждым шагом
                int displayPrecision = Math.Min(28, 5 + step * 2);

                Console.WriteLine($"[{FormatDecimal(oldLow, displayPrecision)} ; {FormatDecimal(oldHigh, displayPrecision)}]");

                foreach (var n in Nodes)
                {
                    decimal low = oldLow + (oldHigh - oldLow) * n.Low;
                    decimal high = oldLow + (oldHigh - oldLow) * n.High;
                    string mark = (n.Symbol == ch) ? "   <= выбран" : "";
                    Console.WriteLine($"{n.Symbol}  {FormatDecimal(low, displayPrecision)} - {FormatDecimal(high, displayPrecision)}{mark}");
                }

                var node = Nodes.Find(x => x.Symbol == ch);
                Range.Low = oldLow + (oldHigh - oldLow) * node.Low;
                Range.High = oldLow + (oldHigh - oldLow) * node.High;

                Console.WriteLine();
            }

            return Range.Low;
        }

        public string Decompress(decimal code, int length)
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine("=== Обратное преобразование ===");
            decimal currentCode = code;

            for (int i = 0; i < length; i++)
            {
                int displayPrecision = Math.Min(28, 10 + i * 2);

                Console.WriteLine($"\nШаг {i + 1}:");
                Console.WriteLine($"Текущее значение кода (K): {FormatDecimal(currentCode, displayPrecision)}");

                // Находим интервал, куда попало число
                Node n = null;
                foreach (var node in Nodes)
                {
                    if (currentCode >= node.Low && currentCode < node.High)
                    {
                        n = node;
                        break;
                    }
                }

                if (n == null)
                {
                    // Для обработки крайнего случая
                    n = Nodes.Last();
                }

                Console.WriteLine($"K попадает в [{FormatDecimal(n.Low, displayPrecision)}; {FormatDecimal(n.High, displayPrecision)}] -> '{n.Symbol}'");

                decimal newCode = (currentCode - n.Low) / (n.High - n.Low);
                Console.WriteLine($"Новый код: (K - n1) / (n2 - n1) = {FormatDecimal(newCode, displayPrecision)}");

                sb.Append(n.Symbol);
                currentCode = newCode;
            }

            return sb.ToString();
        }

        // Вспомогательный метод для форматирования (копия из Program)
        private string FormatDecimal(decimal value, int maxDigits = 15)
        {
            string str = value.ToString();

            if (str.Length > maxDigits + 2)
            {
                int decimalPoint = str.IndexOf('.');
                if (decimalPoint == -1) decimalPoint = str.IndexOf(',');

                if (decimalPoint != -1 && decimalPoint < str.Length - 1)
                {
                    int digitsToKeep = Math.Min(maxDigits, str.Length - decimalPoint - 1);
                    if (digitsToKeep > 0)
                    {
                        str = str.Substring(0, decimalPoint + digitsToKeep + 1);
                    }
                }
                else if (str.Length > maxDigits)
                {
                    str = str.Substring(0, maxDigits);
                }
            }

            return str;
        }
    }

    public class Node
    {
        public char Symbol { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
    }
}