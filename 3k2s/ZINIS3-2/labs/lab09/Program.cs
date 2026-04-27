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

        Console.WriteLine("\nВыберите алгоритм:");
        Console.WriteLine("1 - MD5");
        Console.WriteLine("2 - SHA1");
        Console.WriteLine("3 - SHA256");
        Console.WriteLine("4 - SHA384");
        Console.WriteLine("5 - SHA512");
        Console.Write("Ваш выбор: ");

        string choice = Console.ReadLine() ?? "3";

        HashAlgorithm algorithm = CreateAlgorithm(choice);
        string algorithmName = algorithm.GetType().Name;

        Console.WriteLine($"\nВыбран алгоритм: {algorithmName}");

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

        algorithm.Dispose();
    }

    static HashAlgorithm CreateAlgorithm(string choice)
    {
        return choice switch
        {
            "1" => MD5.Create(),
            "2" => SHA1.Create(),
            "3" => SHA256.Create(),
            "4" => SHA384.Create(),
            "5" => SHA512.Create(),
            _ => SHA256.Create()
        };
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