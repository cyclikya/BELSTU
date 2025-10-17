﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace lab5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var msg = new int[40] { 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0,
                            1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1 };

            Printer.PrintBits("Сообщение: ", msg);

            var matrix = IterativeCode.MsgTo2DimMatrix(msg, 5, 8);
            var checkBits = IterativeCode.CalculateCheckBits(matrix);

            Printer.PrintMatrix("Двумерная матрица:", matrix, false);
            Console.WriteLine();

            int cols = matrix.GetLength(1); 
            int rows = matrix.GetLength(0);

            Console.Write("Горизонтальные паритеты (Xh): ");
            for (int i = cols; i < cols + rows; i++)
                Console.Write(checkBits[i]);
            Console.WriteLine();

            Console.Write("Вертикальные паритеты (Xv): ");
            for (int i = 0; i < cols; i++)
                Console.Write(checkBits[i]);
            Console.WriteLine();

            Console.WriteLine($"Суперпаритет (Xhv): {checkBits[checkBits.Length - 1]}");
            Console.WriteLine();

            Printer.PrintBits("Все контрольные биты Xr= ", checkBits);
            Console.WriteLine("=========================================\n");

            // === Далее начинается часть тестирования ===
            int N1 = 0;
            int N2 = 0;
            int N3 = 0;

            while (true)
            {
                Console.WriteLine("Введите номера битов с ошибками через пробел (Enter — завершить): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                N1++;
                var testMsg = (int[])msg.Clone();
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var p in parts)
                {
                    if (int.TryParse(p, out int pos) && pos >= 0 && pos < testMsg.Length)
                        testMsg[pos] = testMsg[pos] == 0 ? 1 : 0;
                }

                var newMatrix = IterativeCode.MsgTo2DimMatrix(testMsg, 5, 8);
                Printer.PrintMatrix("Матрица с ошибками:", newMatrix, false);

                var foundErrors = IterativeCode.FindErrorPositions(newMatrix, checkBits);

                if (foundErrors.Count > 0)
                {
                    Console.Write("Найдены ошибки в позициях: ");
                    foreach (var pos in foundErrors)
                        Console.Write($"{pos} ");
                    Console.WriteLine();
                }
                else Console.WriteLine("Ошибки не найдены.");

                if (foundErrors.Count == parts.Length)
                    N2++;

                bool allCorrected = foundErrors.All(f => parts.Select(int.Parse).Contains(f));
                if (allCorrected && foundErrors.Count == parts.Length)
                    N3++;

                Console.WriteLine("----------------------------------");
            }

            Console.WriteLine("\nРЕЗУЛЬТАТЫ ЭКСПЕРИМЕНТОВ:");
            Console.WriteLine($"N1 = {N1}  (всего экспериментов)");
            Console.WriteLine($"N2 = {N2}  (правильно определена кратность)");
            Console.WriteLine($"N3 = {N3}  (все ошибки корректно определены)");

            if (N1 > 0)
            {
                Console.WriteLine($"N2/N1 = {(double)N2 / N1:F2}");
                Console.WriteLine($"N3/N1 = {(double)N3 / N1:F2}");
            }

            Console.WriteLine("\nРабота завершена.");
            Console.ReadLine();
        }

        public static class IterativeCode
        {
            public static int[,] MsgTo2DimMatrix(int[] msg)
            {
                int len = (int)Math.Sqrt(msg.Length);
                int height = msg.Length / len;
                return MsgTo2DimMatrix(msg, len, height);
            }

            public static int[,] MsgTo2DimMatrix(int[] msg, int height, int len)
            {
                if (len * height != msg.Length)
                    throw new ArgumentException("Размеры матрицы не соответствуют размерам сообщения");

                int[,] matrix = new int[len, height];
                for (int i = 0; i < len; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        matrix[i, j] = msg[i * height + j];
                    }
                }
                return matrix;
            }

            public static int[] CalculateCheckBits(int[,] matrix)
            {
                int len = matrix.GetLength(1) + matrix.GetLength(0) + 1;
                int[] bits = new int[len];
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    int colSum = 0;
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        colSum += matrix[j, i];
                    }
                    bits[i] = colSum % 2;
                }
                int allSum = 0;
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    int rowSum = 0;
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        rowSum += matrix[i, j];
                    }
                    allSum += rowSum;
                    bits[i + matrix.GetLength(1)] = rowSum % 2;
                }
                for (int i = 0; i < len - 1; i++)
                {
                    allSum += bits[i];
                }
                bits[len - 1] = allSum % 2;
                return bits;
            }

            public static List<int> FindErrorPositions(int[,] matrix, int[] checkBits)
            {
                var checkBitsForMatrix = CalculateCheckBits(matrix);

                var rowMismatch = new List<int>();
                var collMismatch = new List<int>();

                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    if (checkBits[i] != checkBitsForMatrix[i])
                    {
                        collMismatch.Add(i);
                    }
                }
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (checkBits[i + matrix.GetLength(1)] != checkBitsForMatrix[i + matrix.GetLength(1)])
                    {
                        rowMismatch.Add(i);
                    }
                }
                List<int> result = new List<int>();
                foreach (var row in rowMismatch)
                {
                    foreach (var coll in collMismatch)
                    {
                        result.Add(row * matrix.GetLength(1) + coll);
                    }
                }
                return result;
            }
        }

        public static class Printer
        {
            public static void PrintMatrix(string msg, int[,] matrix, bool reverse = true)
            {
                Console.WriteLine($"{msg}");
                for (int i = 0; i < matrix.GetLength(reverse ? 1 : 0); i++)
                {
                    Console.Write("    ");
                    for (int j = 0; j < matrix.GetLength(reverse ? 0 : 1); j++)
                    {
                        Console.Write(reverse ? matrix[j, i] : matrix[i, j]);

                    }
                    Console.WriteLine();
                }

            }
            public static void PrintCheckMatrixH(string msg, int[,] matrix, bool reverse = true)
            {
                int r = matrix.GetLength(1) - 1;
                Console.WriteLine($"{msg}");
                for (int i = 0; i < matrix.GetLength(reverse ? 1 : 0); i++)
                {
                    Console.Write("    ");
                    var rowLen = matrix.GetLength(reverse ? 0 : 1);
                    for (int j = 0; j < rowLen; j++)
                    {
                        Console.Write(reverse ? matrix[j, i] : matrix[i, j]);
                        if (j == rowLen - 1 - r) Console.Write(" | ");
                        //if(j < rowLen - 1)
                        //    Console.Write(", ");
                    }
                    Console.WriteLine();
                }

            }
            public static void PrintCheckMatrix(string msg, int[,] matrix, bool reverse = true)
            {
                int r = matrix.GetLength(1);
                Console.WriteLine($"{msg}");
                for (int i = 0; i < matrix.GetLength(reverse ? 1 : 0); i++)
                {
                    Console.Write("    ");
                    var rowLen = matrix.GetLength(reverse ? 0 : 1);
                    for (int j = 0; j < rowLen; j++)
                    {
                        Console.Write(reverse ? matrix[j, i] : matrix[i, j]);
                        if (j == rowLen - 1 - r) Console.Write(" | ");
                        //if(j < rowLen - 1)
                        //    Console.Write(", ");
                    }
                    Console.WriteLine();
                }

            }
            public static void PrintBitsAndCheckBits(string msg, int[] bits, int k)
            {
                Console.Write(msg);
                int r = (int)Math.Log(k, 2) + 1;
                for (int i = 0; i < k; i++)
                {
                    Console.Write(bits[i]);
                }
                Console.Write('.');
                for (int i = 0; i < r; i++)
                {
                    Console.Write(bits[i + k]);
                }
                Console.WriteLine();
            }
            public static void PrintBits(string msg, int[] bits)
            {
                Console.Write(msg);
                for (int i = 0; i < bits.Length; i++)
                    Console.Write(bits[i]);
                Console.WriteLine();
            }
        }
    }







}