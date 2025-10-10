using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class Program
{
    static Random rng = new Random();

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 1. Чтение исходных данных
        string inputPath = "input.txt";
        string text;
        if (File.Exists(inputPath))
        {
            text = File.ReadAllText(inputPath, Encoding.UTF8).Trim();
            if (string.IsNullOrEmpty(text)) text = "Hi!";
            Console.WriteLine($"Исходный текст (из файла): \"{text}\"");
        }
        else
        {
            text = "Hi!";
            Console.WriteLine($"Файл '{inputPath}' не найден. Используется текст по умолчанию: \"{text}\"");
        }

        // 1.1 Преобразование текста в двоичный вид (8 бит на символ)
        string binary = TextToBinary(text);
        Console.WriteLine($"\nДвоичное представление ({binary.Length} бит):");
        Console.WriteLine(binary);

        if (binary.Length < 16)
        {
            Console.WriteLine("\nДлина сообщения менее 16 бит. Увеличение длины до 16 бит.");
            while (binary.Length < 16)
            {
                binary += binary;
            }
            binary = binary.Substring(0, 16);
        }

        // 2. Вычисление параметров кода
        int k = binary.Length;
        int r = CalculateRequiredR(k);
        int n = k + r;

        Console.WriteLine("\nПАРАМЕТРЫ КОДА");
        Console.WriteLine($"Количество информационных бит (k): {k}");
        Console.WriteLine($"Количество проверочных бит (r): {r}");
        Console.WriteLine($"Общая длина кодового слова (n = k + r): {n}");
        Console.WriteLine($"Условие 2^r ≥ k + r + 1: 2^{r} = {Math.Pow(2, r)}, k + r + 1 = {k + r + 1}");

        // 3. Построение проверочной матрицы H
        Console.WriteLine("\nМАТРИЦА H (r × n)");
        int[,] H = BuildHammingMatrix(k, r);
        PrintMatrix(H, "H");

        Console.WriteLine("\nПроверка уникальности столбцов матрицы H:");
        bool unique = CheckSyndromeUniqueness(H);
        if (!unique)
        {
            Console.WriteLine("Обнаружены дублирующиеся синдромы. Завершение программы.");
            return;
        }

        // 4. Формирование кодового слова
        int[] Xk = binary.Select(c => c == '1' ? 1 : 0).ToArray();
        int[] Xr = ComputeParityFromP(H, Xk, r, k);
        int[] Xn = new int[n];
        Array.Copy(Xk, 0, Xn, 0, k);
        Array.Copy(Xr, 0, Xn, k, r);

        Console.WriteLine("\nФОРМИРОВАНИЕ КОДОВОГО СЛОВА");
        Console.WriteLine($"Информационные биты Xk: {string.Join("", Xk)}");
        Console.WriteLine($"Проверочные биты Xr:     {string.Join("", Xr)}");
        Console.WriteLine($"Полное кодовое слово Xn: {string.Join("", Xn)}");

        // Проверка корректности кодирования
        int[] syndromeCheck = MultiplyHbyVector(H, Xn, r, n);
        Console.WriteLine("\nПроверка корректности кодирования (синдром H*Xn):");
        Console.WriteLine(string.Join("", syndromeCheck));
        if (syndromeCheck.All(b => b == 0))
            Console.WriteLine("Результат: синдром равен нулю. Кодирование выполнено корректно.");
        else
            Console.WriteLine("Результат: обнаружено несоответствие. Кодирование выполнено с ошибкой.");

        // 5. Моделирование ошибок
        Console.WriteLine("\nЭКСПЕРИМЕНТЫ С ОШИБКАМИ");
        var scenarios = new (string name, int count)[] {
            ("Без ошибок", 0),
            ("Одна ошибка", 1),
            ("Две ошибки", 2)
        };

        foreach (var (name, count) in scenarios)
        {
            Console.WriteLine($"\nСценарий: {name}");
            Console.WriteLine(new string('-', 60));

            int[] Yn = (int[])Xn.Clone();
            List<int> errorPositions = new();

            if (count > 0)
            {
                errorPositions = GenerateUniqueRandomPositions(count, n);
                Console.WriteLine($"Ошибки внесены в позиции: {string.Join(", ", errorPositions.Select(p => p + 1))}");
                foreach (int p in errorPositions)
                    Yn[p] = 1 - Yn[p];
            }

            Console.WriteLine($"Принятое слово Yn: {string.Join("", Yn)}");

            int[] Yk = Yn.Take(k).ToArray();
            int[] YrActual = Yn.Skip(k).Take(r).ToArray();
            int[] YrRecalc = ComputeParityFromP(H, Yk, r, k);

            Console.WriteLine($"Yr (принятые проверочные биты): {string.Join("", YrActual)}");
            Console.WriteLine($"Yr' (пересчитанные из Yk):      {string.Join("", YrRecalc)}");

            int[] S = MultiplyHbyVector(H, Yn, r, n);
            Console.WriteLine($"Синдром S = H * Yn: {string.Join("", S)}");

            int[] En = new int[n];
            int[] corrected = (int[])Yn.Clone();

            if (S.All(b => b == 0))
            {
                if (count == 0)
                    Console.WriteLine("Ошибок не обнаружено.");
                else
                    Console.WriteLine("Синдром нулевой, но ошибки имеются. Возможна маскировка.");
            }
            else
            {
                int errorPos = FindErrorPosition(H, S);
                if (errorPos != -1)
                {
                    En[errorPos] = 1;
                    corrected[errorPos] = 1 - corrected[errorPos];
                    Console.WriteLine($"Обнаружена ошибка в позиции {errorPos + 1}. Исправление выполнено.");
                }
                else
                {
                    Console.WriteLine("Ошибка не может быть локализована (возможно, множественная ошибка).");
                }
            }

            Console.WriteLine($"Вектор ошибки En: {string.Join("", En)}");
            Console.WriteLine($"Исправленное слово: {string.Join("", corrected)}");

            bool identical = corrected.SequenceEqual(Xn);
            Console.WriteLine(identical
                ? "Результат: кодовое слово восстановлено корректно."
                : "Результат: восстановление не удалось.");
        }
    }

    // ---------- Вспомогательные методы ----------

    static string TextToBinary(string text)
    {
        var sb = new StringBuilder();
        foreach (char c in text)
            sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
        return sb.ToString();
    }

    static int CalculateRequiredR(int k)
    {
        int r = 1;
        while ((1 << r) < (k + r + 1)) r++;
        return r;
    }

    static int[,] BuildHammingMatrix(int k, int r)
    {
        int n = k + r;
        int[,] H = new int[r, n];

        // Единичная матрица для проверочных битов (правая часть)
        for (int i = 0; i < r; i++)
            H[i, k + i] = 1;

        // Формирование левой части (информационные биты)
        var available = new List<int[]>();
        for (int num = 1; num < (1 << r); num++)
        {
            if ((num & (num - 1)) == 0) continue; // исключаем степени двойки
            int[] v = new int[r];
            for (int b = 0; b < r; b++)
                v[b] = (num >> (r - 1 - b)) & 1;
            available.Add(v);
        }

        for (int col = 0; col < k; col++)
        {
            int[] colVec;
            if (col < available.Count) colVec = available[col];
            else
            {
                colVec = GenerateUniqueRandomVector(H, r, n);
            }

            for (int i = 0; i < r; i++)
                H[i, col] = colVec[i];
        }

        return H;
    }

    static int[] GenerateUniqueRandomVector(int[,] H, int r, int n)
    {
        while (true)
        {
            int[] tryV = new int[r];
            for (int i = 0; i < r; i++)
                tryV[i] = rng.Next(0, 2);
            if (tryV.Sum() < 2) continue;

            bool unique = true;
            for (int j = 0; j < n; j++)
            {
                bool equal = true;
                for (int i = 0; i < r; i++)
                    if (H[i, j] != tryV[i]) { equal = false; break; }
                if (equal) { unique = false; break; }
            }
            if (unique) return tryV;
        }
    }

    static void PrintMatrix(int[,] M, string name)
    {
        int r = M.GetLength(0);
        int n = M.GetLength(1);
        Console.WriteLine($"{name} ({r} x {n}):");
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write(M[i, j] + " ");
            Console.WriteLine();
        }
    }

    static bool CheckSyndromeUniqueness(int[,] H)
    {
        int r = H.GetLength(0);
        int n = H.GetLength(1);
        var map = new HashSet<string>();
        for (int j = 0; j < n; j++)
        {
            string col = string.Concat(Enumerable.Range(0, r).Select(i => H[i, j].ToString()));
            if (!map.Add(col))
            {
                Console.WriteLine($"Дублирующийся синдром обнаружен в столбце {j + 1}.");
                return false;
            }
        }
        Console.WriteLine("Все синдромы уникальны.");
        return true;
    }

    static int[] MultiplyHbyVector(int[,] H, int[] v, int r, int n)
    {
        int[] result = new int[r];
        for (int i = 0; i < r; i++)
        {
            int sum = 0;
            for (int j = 0; j < n; j++)
                sum ^= (H[i, j] & v[j]);
            result[i] = sum & 1;
        }
        return result;
    }

    static int[] ComputeParityFromP(int[,] H, int[] Xk, int r, int k)
    {
        int[] Xr = new int[r];
        for (int i = 0; i < r; i++)
        {
            int s = 0;
            for (int j = 0; j < k; j++)
                s ^= (H[i, j] & Xk[j]);
            Xr[i] = s & 1;
        }
        return Xr;
    }

    static int FindErrorPosition(int[,] H, int[] S)
    {
        int r = H.GetLength(0);
        int n = H.GetLength(1);
        for (int j = 0; j < n; j++)
        {
            bool equal = true;
            for (int i = 0; i < r; i++)
            {
                if (H[i, j] != S[i]) { equal = false; break; }
            }
            if (equal) return j;
        }
        return -1;
    }

    static List<int> GenerateUniqueRandomPositions(int count, int n)
    {
        var list = Enumerable.Range(0, n).ToList();
        var res = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int idx = rng.Next(list.Count);
            res.Add(list[idx]);
            list.RemoveAt(idx);
        }
        return res;
    }
}
