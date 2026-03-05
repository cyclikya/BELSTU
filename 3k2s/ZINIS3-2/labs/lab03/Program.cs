using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab03
{
    class Program
    {
        static string basePath = @"C:\General\BELSTU\3k2s\ZINIS3-2\labs\lab03\";
        static string nameKey = "Виолетта";
        static string surnameKey = "Угоренко";

        static int blockSize = nameKey.Length * surnameKey.Length;

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.Write("Введите имя файла: ");
            string fileName = Console.ReadLine();
            string fullPath = Path.Combine(basePath, fileName);

            if (!File.Exists(fullPath))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            string text = File.ReadAllText(fullPath, Encoding.UTF8);

            Console.WriteLine("1 – Маршрутная перестановка");
            Console.WriteLine("2 – Множественная перестановка");
            int method = int.Parse(Console.ReadLine());

            Console.WriteLine("1 – Зашифровать");
            Console.WriteLine("2 – Расшифровать");
            int action = int.Parse(Console.ReadLine());

            string result = "";

            if (method == 1)
            {
                Console.Write("Введите количество строк: ");
                int rows = int.Parse(Console.ReadLine());

                Console.Write("Введите количество столбцов: ");
                int cols = int.Parse(Console.ReadLine());

                if (action == 1)
                    result = SpiralEncrypt(text, rows, cols);
                else
                    result = SpiralDecrypt(text, rows, cols);
            }
            else
            {
                if (action == 1)
                    result = MultipleEncrypt(text);
                else
                    result = MultipleDecrypt(text);
            }

            string outputFile = action == 1 ? "decrypt.txt" : "encrypt.txt";
            File.WriteAllText(Path.Combine(basePath, outputFile), result, Encoding.UTF8);

            Console.WriteLine($"\nГотово! Результат сохранён в {outputFile}");
        }

        // ================= СПИРАЛЬ ПРОТИВ ЧАСОВОЙ =================

        static string SpiralEncrypt(string text, int rows, int cols)
        {
            char[,] table = new char[rows, cols];
            int index = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    table[i, j] = index < text.Length ? text[index++] : '#';

            PrintTable(table, rows, cols);

            return ReadSpiralCounterClockwise(table, rows, cols);
        }

        static string SpiralDecrypt(string text, int rows, int cols)
        {
            char[,] table = new char[rows, cols];
            int index = 0;

            int top = 0, bottom = rows - 1;
            int left = 0, right = cols - 1;

            while (top <= bottom && left <= right)
            {
                // вниз
                for (int i = top; i <= bottom; i++)
                    table[i, left] = text[index++];
                left++;

                // вправо
                for (int i = left; i <= right; i++)
                    table[bottom, i] = text[index++];
                bottom--;

                if (left <= right)
                {
                    // вверх
                    for (int i = bottom; i >= top; i--)
                        table[i, right] = text[index++];
                    right--;
                }

                if (top <= bottom)
                {
                    // влево
                    for (int i = right; i >= left; i--)
                        table[top, i] = text[index++];
                    top++;
                }
            }

            PrintTable(table, rows, cols);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (table[i, j] != '#')
                        sb.Append(table[i, j]);

            return sb.ToString();
        }

        static string ReadSpiralCounterClockwise(char[,] table, int rows, int cols)
        {
            StringBuilder sb = new StringBuilder();

            int top = 0, bottom = rows - 1;
            int left = 0, right = cols - 1;

            while (top <= bottom && left <= right)
            {
                for (int i = top; i <= bottom; i++)
                    sb.Append(table[i, left]);
                left++;

                for (int i = left; i <= right; i++)
                    sb.Append(table[bottom, i]);
                bottom--;

                if (left <= right)
                {
                    for (int i = bottom; i >= top; i--)
                        sb.Append(table[i, right]);
                    right--;
                }

                if (top <= bottom)
                {
                    for (int i = right; i >= left; i--)
                        sb.Append(table[top, i]);
                    top++;
                }
            }

            return sb.ToString();
        }

        // ================= МНОЖЕСТВЕННАЯ ПЕРЕСТАНОВКА БЛОКАМИ =================

        static string MultipleEncrypt(string text)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < text.Length; i += blockSize)
            {
                string block = text.Substring(i, Math.Min(blockSize, text.Length - i));
                block = block.PadRight(blockSize, '#');

                Console.WriteLine("\n=== Новый блок ===");
                Console.WriteLine(block);

                block = PermuteBlock(block, nameKey);
                block = PermuteBlock(block, surnameKey);

                result.Append(block);
            }

            return result.ToString();
        }

        static string MultipleDecrypt(string text)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < text.Length; i += blockSize)
            {
                string block = text.Substring(i, blockSize);

                block = ReversePermuteBlock(block, surnameKey);
                block = ReversePermuteBlock(block, nameKey);

                result.Append(block);
            }

            return result.ToString().TrimEnd('#');
        }

        static string PermuteBlock(string block, string key)
        {
            int size = key.Length; // 8
            char[,] table = new char[size, size];

            int index = 0;

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    table[i, j] = block[index++];

            PrintTable(table, size);

            var sorted = key
                .Select((c, i) => new { Char = c, Index = i })
                .OrderBy(x => x.Char)
                .ToList();

            Console.WriteLine("Сортировка:");
            foreach (var s in sorted)
                Console.WriteLine($"{s.Char} -> {s.Index}");

            StringBuilder sb = new StringBuilder();

            foreach (var col in sorted)
                for (int i = 0; i < size; i++)
                    sb.Append(table[i, col.Index]);

            return sb.ToString();
        }

        static string ReversePermuteBlock(string block, string key)
        {
            int size = key.Length;
            char[,] table = new char[size, size];

            var sorted = key
                .Select((c, i) => new { Char = c, Index = i })
                .OrderBy(x => x.Char)
                .ToList();

            int index = 0;

            foreach (var col in sorted)
                for (int i = 0; i < size; i++)
                    table[i, col.Index] = block[index++];

            PrintTable(table, size);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    sb.Append(table[i, j]);

            return sb.ToString();
        }

        static void PrintTable(char[,] table, int size)
        {
            Console.WriteLine("Таблица:");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                    Console.Write(table[i, j] + " ");
                Console.WriteLine();
            }
        }

        static void PrintTable(char[,] table, int rows, int cols)
        {
            Console.WriteLine("\nТаблица:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    Console.Write(table[i, j] + " ");
                Console.WriteLine();
            }
        }
    }
}