using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Лабораторная работа №9");
        Console.WriteLine("Исследование криптографических хеш-функций\n");

        Console.WriteLine("Введите сообщение:");
        string text = Console.ReadLine() ?? "";

        Console.WriteLine("\nИспользуется алгоритм: SHA256");

        using (SHA256 algorithm = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);

            Stopwatch stopwatch = Stopwatch.StartNew();
            byte[] hashBytes = algorithm.ComputeHash(inputBytes);
            stopwatch.Stop();

            string hash = BytesToHex(hashBytes);

            Console.WriteLine("\nИсходный текст:");
            Console.WriteLine(text);

            Console.WriteLine("\nХеш:");
            Console.WriteLine(hash);

            Console.WriteLine($"\nДлина хеша: {hashBytes.Length * 8} бит");
            Console.WriteLine($"Время вычисления: {stopwatch.Elapsed.TotalMilliseconds} мс");

            Console.WriteLine("\nПроверка лавинного эффекта");
            string changedText = text.Length > 0
                ? text.Substring(0, text.Length - 1) + (text[^1] == 'а' ? 'б' : 'а')
                : "а";

            byte[] changedHashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(changedText));
            string changedHash = BytesToHex(changedHashBytes);

            int differentBits = CountDifferentBits(hashBytes, changedHashBytes);
            int totalBits = hashBytes.Length * 8;

            Console.WriteLine("Измененный текст:");
            Console.WriteLine(changedText);

            Console.WriteLine("\nХеш измененного текста:");
            Console.WriteLine(changedHash);

            Console.WriteLine($"\nИзменилось бит: {differentBits} из {totalBits}");
            Console.WriteLine($"Процент изменения: {(double)differentBits / totalBits * 100:F2}%");

            Console.WriteLine("\nОценка вероятности коллизии по парадоксу дня рождения");
            Console.Write("Введите количество сообщений N: ");
            double n = Convert.ToDouble(Console.ReadLine());

            int hashLength = hashBytes.Length * 8;
            double probability = CollisionProbability(n, hashLength);

            Console.WriteLine($"Вероятность коллизии примерно: {probability:E6}");
        }
    }

    static string BytesToHex(byte[] bytes)
    {
        StringBuilder result = new StringBuilder();

        foreach (byte b in bytes)
        {
            result.Append(b.ToString("x2"));
        }

        return result.ToString();
    }

    static int CountDifferentBits(byte[] first, byte[] second)
    {
        int count = 0;

        for (int i = 0; i < first.Length; i++)
        {
            byte xor = (byte)(first[i] ^ second[i]);

            while (xor != 0)
            {
                count += xor & 1;
                xor >>= 1;
            }
        }

        return count;
    }

    static double CollisionProbability(double n, int hashLength)
    {
        double m = Math.Pow(2, hashLength);
        return 1 - Math.Exp(-(n * (n - 1)) / (2 * m));
    }
}