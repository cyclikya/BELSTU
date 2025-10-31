using Microsoft.VisualBasic;
using System;

namespace Laba7
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Лабораторная работа №7 (вариант 10)");
            Console.WriteLine("Тема: Перемежение и деперемежение данных с использованием кода Хемминга\n");

            int k = 5;            
            int columns = 7;      
            int messageBytes = 14;
            int messageBits = messageBytes * 8;

            Console.WriteLine($"\n============ ДАННЫЕ");
            string msg = "0001000000001011111110100000110110100001110011111001100010100011100111110010101100101111010100000001101010000011";
            //string msg = GenerateRandomBinaryString(messageBits);
            Console.WriteLine($"Исходное сообщение ({messageBits} бит):");
            Console.WriteLine(msg + "\n");

            Console.WriteLine($"\n============ КОДЕР");
            var words = SplitMessage(msg, k);
            Console.WriteLine($"Разбиение на {words.Length} кодовых слов по {k} бит:");
            foreach (var w in words) Console.WriteLine(w);
            Console.WriteLine();

            int r = HammingR(k);
            int n = k + r;
            int[,] checkMatrix = BuildCheckMatrix(n);
            Console.WriteLine("Проверочная матрица H:");
            PrintMatrix(checkMatrix);
            Console.WriteLine();

            int[][] codewords = new int[words.Length][];
            for (int i = 0; i < words.Length; i++)
            {
                codewords[i] = EncodeHamming(words[i], checkMatrix, k);
            }

            Console.WriteLine("Кодовые слова:");
            foreach (var w in codewords) PrintArray(w);

            Console.WriteLine($"\n============ ПЕРЕМЕЖИТЕЛЬ");

            int[] fullData = codewords.SelectMany(x => x).ToArray();
            Console.WriteLine($"Кодовая комбинация: {fullData}");
            int rows = (int)Math.Ceiling((double)fullData.Length / columns);
            Console.WriteLine($"Форма матрицы перемежителя: {rows} строк * {columns} столбцов\n");

            Console.WriteLine("Матрица перемежения (до перестановки):");
            PrintInterleaverMatrix(fullData, rows, columns);

            int[] interleaved = InterleaveFlat(fullData, columns);

            Console.WriteLine("\nМатрица после перемежения (по столбцам):");
            PrintDeinterleaverMatrix(interleaved, rows, columns);

            Console.WriteLine("\nСтрока после перемежения:");
            PrintArray(interleaved);

            foreach (int errorLen in new int[] { 4, 6, 8 })
            {
                Console.WriteLine($"\n============ МОДЕЛИРОВАНИЕ ПАКЕТНОЙ ОШИБКИ длиной {errorLen} бит ===");
                int[] corrupted = (int[])interleaved.Clone();
                int errorPos = new Random().Next(0, corrupted.Length - errorLen);
                for (int i = errorPos; i < errorPos + errorLen; i++)
                    corrupted[i] ^= 1;

                Console.WriteLine($"Ошибка с позиции {errorPos}, длина {errorLen}");
                Console.WriteLine("Строка с ошибками:");
                PrintArray(corrupted);

                string infoErrorBits = RemoveParityBits(corrupted, k, r);
                Console.WriteLine("\nСтрока с ошибками (только информационные биты):");
                Console.WriteLine(infoErrorBits);

                int[] deinterleaved = DeinterleaveFlat(corrupted, columns);
                int[][] received = new int[codewords.Length][];
                for (int i = 0; i < codewords.Length; i++)
                    received[i] = deinterleaved.Skip(i * n).Take(n).ToArray();

                for (int i = 0; i < received.Length; i++)
                    received[i] = CorrectHamming(received[i], checkMatrix, k);

                string restored = string.Join("", DecodeWords(received, k));
                Console.WriteLine("\nВосстановленное сообщение:");
                Console.WriteLine(restored);

                int diff = CountBitDifferences(msg, restored);
                Console.WriteLine($"Количество отличных битов: {diff}\n");
            }

            Console.WriteLine($"\n============ АНАЛИЗ ГРУППОВЫХ ОШИБОК");
            int experiments = 40;
            double totalOverallDiff = 0;
            int totalOverallRuns = 0;

            foreach (int errorLen in new int[] { 4, 6, 8 })
            {
                double totalDiff = 0;

                for (int exp = 0; exp < experiments; exp++)
                {
                    int[] corrupted = (int[])interleaved.Clone();
                    int errorPos = new Random().Next(0, corrupted.Length - errorLen);
                    for (int i = errorPos; i < errorPos + errorLen; i++)
                        corrupted[i] ^= 1;

                    int[] deinterleaved = DeinterleaveFlat(corrupted, columns);

                    int[][] received = new int[codewords.Length][];
                    for (int i = 0; i < codewords.Length; i++)
                        received[i] = deinterleaved.Skip(i * n).Take(n).ToArray();

                    for (int i = 0; i < received.Length; i++)
                        received[i] = CorrectHamming(received[i], checkMatrix, k);

                    string restored = string.Join("", DecodeWords(received, k));
                    int diff = CountBitDifferences(msg, restored);
                    totalDiff += diff;
                }

                double percentErrors = 100.0 * ((totalDiff / experiments) / msg.Length);
                Console.WriteLine($"Средний процент неисправленных бит при пакетной ошибке длиной {errorLen} бит (по {experiments} попыткам): {percentErrors:F2}%");

                totalOverallDiff += totalDiff;
                totalOverallRuns += experiments;
            }
            double avgAll = totalOverallDiff / totalOverallRuns;
            Console.WriteLine($"\nСредняя эффективность восстановления: {100.0 * (1 - avgAll / msg.Length):F2}%");
        }

        // --- Код Хемминга ---
        static int HammingR(int k)
        {
            int r = 0;
            while (Math.Pow(2, r) < k + r + 1) r++;
            return r;
        }

        static int[,] BuildCheckMatrix(int n)
        {
            int r = (int)Math.Log2(n) + 1;
            int[,] H = new int[r, n];
            for (int col = 1; col <= n; col++)
            {
                string bin = Convert.ToString(col, 2).PadLeft(r, '0');
                for (int row = 0; row < r; row++)
                    H[row, col - 1] = bin[row] == '1' ? 1 : 0;
            }
            return H;
        }

        static int[] EncodeHamming(string bits, int[,] H, int k)
        {
            int r = H.GetLength(0);
            int n = k + r;
            int[] data = new int[n];
            int j = 0;
            for (int i = 1; i <= n; i++)
                if (!IsPowerOfTwo(i))
                    data[i - 1] = bits[j++] - '0';

            for (int i = 0; i < r; i++)
            {
                int parity = 0;
                for (int j2 = 0; j2 < n; j2++)
                    parity ^= H[i, j2] * data[j2];
                data[(int)Math.Pow(2, i) - 1] = parity % 2;
            }
            return data;
        }

        static int[] CorrectHamming(int[] code, int[,] H, int k)
        {
            int r = H.GetLength(0);
            int n = k + r;
            int[] syndrome = new int[r];
            for (int i = 0; i < r; i++)
            {
                int s = 0;
                for (int j = 0; j < n; j++)
                    s ^= H[i, j] * code[j];
                syndrome[i] = s % 2;
            }
            int errorPos = 0;
            for (int i = 0; i < r; i++)
                if (syndrome[i] == 1)
                    errorPos += (int)Math.Pow(2, i);

            if (errorPos > 0 && errorPos <= n)
                code[errorPos - 1] ^= 1;

            return code;
        }

        static bool IsPowerOfTwo(int x) => (x & (x - 1)) == 0;

        // --- Перемежение / деперемежение ---
        static int[] InterleaveFlat(int[] data, int columns)
        {
            int rows = (int)Math.Ceiling((double)data.Length / columns);
            int[,] matrix = new int[rows, columns];

            int idx = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (idx < data.Length)
                        matrix[i, j] = data[idx++];

            int[] result = new int[data.Length];
            idx = 0;
            for (int j = 0; j < columns; j++)
                for (int i = 0; i < rows; i++)
                    if (idx < data.Length)
                        result[idx++] = matrix[i, j];

            return result;
        }

        static int[] DeinterleaveFlat(int[] data, int columns)
        {
            int rows = (int)Math.Ceiling((double)data.Length / columns);
            int[,] matrix = new int[rows, columns];

            int idx = 0;
            for (int j = 0; j < columns; j++)
                for (int i = 0; i < rows; i++)
                    if (idx < data.Length)
                        matrix[i, j] = data[idx++];

            int[] result = new int[data.Length];
            idx = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (idx < data.Length)
                        result[idx++] = matrix[i, j];

            return result;
        }

        // --- Вспомогательные функции ---
        static string GenerateRandomBinaryString(int length)
        {
            Random rnd = new Random();
            char[] bits = new char[length];
            for (int i = 0; i < length; i++)
                bits[i] = rnd.Next(2) == 0 ? '0' : '1';
            return new string(bits);
        }

        static string[] SplitMessage(string msg, int size)
        {
            int count = (int)Math.Ceiling((double)msg.Length / size);
            string[] parts = new string[count];
            for (int i = 0; i < count; i++)
            {
                int start = i * size;
                int len = Math.Min(size, msg.Length - start);
                parts[i] = msg.Substring(start, len).PadRight(size, '0');
            }
            return parts;
        }

        static string[] DecodeWords(int[][] blocks, int k)
        {
            string[] res = new string[blocks.Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                string s = "";
                int n = blocks[i].Length;
                for (int j = 1; j <= n; j++)
                    if (!IsPowerOfTwo(j))
                        s += blocks[i][j - 1].ToString();
                res[i] = s.Substring(0, k);
            }
            return res;
        }

        static void PrintMatrix(int[,] M)
        {
            int r = M.GetLength(0);
            int c = M.GetLength(1);
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                    Console.Write(M[i, j]);
                Console.WriteLine();
            }
        }

        static void PrintArray(int[] arr)
        {
            foreach (int b in arr) Console.Write(b);
            Console.WriteLine();
        }

        static int CountBitDifferences(string a, string b)
        {
            int count = 0;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) count++;
            return count;
        }
        static void PrintInterleaverMatrix(int[] data, int rows, int columns)
        {
            int idx = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (idx < data.Length)
                        Console.Write(data[idx]);
                    else
                        Console.Write(" ");
                    idx++;
                }
                Console.WriteLine();
            }
        }

        static void PrintDeinterleaverMatrix(int[] data, int rows, int columns)
        {
            int[,] matrix = new int[rows, columns];
            int idx = 0;
            for (int j = 0; j < columns; j++)
                for (int i = 0; i < rows; i++)
                    if (idx < data.Length)
                        matrix[i, j] = data[idx++];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                    Console.Write(matrix[i, j]);
                Console.WriteLine();
            }
        }
        static string RemoveParityBits(int[] codedData, int k, int r)
        {
            int n = k + r;
            int totalBlocks = codedData.Length / n;
            string result = "";

            for (int block = 0; block < totalBlocks; block++)
            {
                for (int i = 1; i <= n; i++)
                {
                    if (!IsPowerOfTwo(i))
                    {
                        int bit = codedData[block * n + (i - 1)];
                        result += bit.ToString();
                    }
                }
            }

            return result;
        }

    }
}