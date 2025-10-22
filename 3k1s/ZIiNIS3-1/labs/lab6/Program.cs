using System;
using System.Collections.Generic;
using System.Linq;

namespace lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            // x^6 + x^3 + 1
            string Xk = "1100001";
            string Xr = "1000011";

            int k = Xk.Length;
            int n = 13;
            int r = n - k;

            int[] masXk = new int[k];
            StrInMas(masXk, Xk);

            int[] masXr = new int[Xr.Length];
            StrInMas(masXr, Xr);

            Console.WriteLine($"Сообщение: {Xk}");
            Console.WriteLine($"Порождающий полином: {Xr}");
            Console.WriteLine($"Длина сообщения: {k}");
            Console.WriteLine($"Кол-во проверочных символов: {r}");
            Console.WriteLine($"Длина сообщения из k={k} символов формируется блок n={n}  символов. ");

            int[,] generationMatrix = new int[k, n];
            CreateGenerationMatrix(generationMatrix, masXr, k, n);

            Console.WriteLine("\nПорождающая матрица:");
            OutMatrix(generationMatrix, k, n);

            CreateCanonicalMatrix(generationMatrix, k, n);

            Console.WriteLine("\nКаноническая матрица:");
            OutMatrix(generationMatrix, k, n);

            int[,] checkMatrix = new int[n, r];
            CreateCheckMatrix(checkMatrix, generationMatrix, k, n);

            Console.WriteLine("\nПроверочная матрица:");
            OutMatrix(checkMatrix, n, r);

            // формируем исходное кодовое слово (masXn = m(x)*x^r, затем добавим остаток)
            int[] masXn = new int[n];
            Shift(masXn, masXk, r);

            // вычисляем остаток (проверочные биты) без печати промежуточных шагов
            int[] residue = ComputeResidueSilent(masXn, masXr);
            // поместим остаток в последние r бит
            for (int j = 0; j < r; j++)
            {
                masXn[k + j] = residue[k + j];
            }

            Console.WriteLine("\nИтоговое кодовое слово (c):");
            OutMass(masXn);

            // Выполним задания: для ошибок 0,1,2 — сгенерировать Yn, вычислить синдром, проанализировать и попытаться исправить одиночную
            Random rnd = new Random();

            for (int errorsCount = 0; errorsCount <= 2; errorsCount++)
            {
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Тест: cгенерировать {errorsCount} ошибок (случайно)\n");
                // копия кодового слова
                int[] Yn = new int[n];
                Array.Copy(masXn, Yn, n);

                // сгенерировать уникальные позиции ошибок
                var pos = new HashSet<int>();
                while (pos.Count < errorsCount)
                {
                    int p = rnd.Next(0, n); // 0..n-1
                    pos.Add(p);
                }

                if (pos.Count != 0)
                {
                    Console.WriteLine($"Позиции ошибок: {string.Join(", ", pos.OrderBy(x => x))}");
                }

                // унарный вектор ошибок En
                int[] En = new int[n];
                foreach (var p in pos)
                {
                    En[p] = 1;
                    Yn[p] = (Yn[p] + 1) % 2; // инвертировать бит
                }

                Console.Write("Сгенерированное слово Yn: ");
                OutMassInline(Yn);

                // вычислим синдром (остаток) для Yn
                int[] YnCopy = new int[n];
                Array.Copy(Yn, YnCopy, n);
                int[] residueYn = ComputeResidueSilent(YnCopy, masXr);

                // синдром — последние r бит остатка (в твоей реализации остаток хранится в последних r позициях массива после деления)
                int[] syndrome = new int[r];
                int kLocal = k;
                for (int j = 0; j < r; j++)
                    syndrome[j] = residueYn[kLocal + j];

                Console.Write("\nСиндром (последние r бит остатка): ");
                OutMassInline(syndrome);
                Console.WriteLine();

                // анализ синдрома: совпадение со строкой H?
                int matchIndex = -1;
                for (int i = 0; i < n; i++)
                {
                    int match = 0;
                    for (int j = 0; j < r; j++)
                        if (checkMatrix[i, j] == syndrome[j]) match++;
                    if (match == r)
                    {
                        matchIndex = i;
                        break;
                    }
                }

                if (matchIndex == -1)
                {
                    if (errorsCount == 0 && syndrome.All(b => b == 0))
                    {
                        Console.WriteLine("Синдром нулевой — ошибок не обнаружено (как и ожидалось).");
                    }
                    else
                    {
                        Console.WriteLine("Синдром не совпал с какой-либо строкой H. Либо 2+ ошибки, либо комбинация, не позволяющая однозначно локализовать одиночную ошибку.");
                        // Покажем что происходит, если попытаться применить тот же механизм (для демонстрации)
                        Console.WriteLine("Попытка локализации одиночной ошибки — неудачна.");
                    }
                }
                else
                {
                    Console.WriteLine($"Синдром совпал со строкой H для позиции i = {matchIndex} (0-based).");
                    // сформируем вектор ошибки найденной позиции and исправим
                    int[] E_found = new int[n];
                    E_found[matchIndex] = 1;
                    Console.Write("Найденный вектор ошибки (по H): ");
                    OutMassInline(E_found);
                    // исправим Yn
                    int[] corrected = new int[n];
                    Array.Copy(Yn, corrected, n);
                    corrected[matchIndex] = (corrected[matchIndex] + 1) % 2;
                    Console.Write("\n\nКодовое слово после исправления предполагаемой одиночной ошибки: ");
                    OutMassInline(corrected);
                    Console.WriteLine();

                    // проверка: стал ли результат равен исходному кодовому слову masXn
                    bool success = corrected.SequenceEqual(masXn);
                    Console.WriteLine(success ? "Исправлено успешно (совпадает с исходным кодовым словом)." : "Исправление не привело к исходному кодовому слову.");
                }
            }

            Console.WriteLine(new string('-', 60));
            Console.WriteLine("Тесты завершены. Нажмите любую клавишу для выхода.");
            Console.ReadKey();
        }

        // --- Вспомогательные функции и исходные функции (с некоторыми дополнениями) ---

        // Silent residue computation: делим masXn на masXr и возвращаем массив n с остатком в последних r позициях.
        public static int[] ComputeResidueSilent(int[] masXn, int[] masXr)
        {
            int n = masXn.Length;
            int m = masXr.Length;
            int end = n - m + 1;
            int[] work = new int[n];
            Array.Copy(masXn, work, n);

            for (int i = 0; i < end; i++)
            {
                if (work[i] == 1)
                {
                    for (int j = 0; j < m; j++)
                    {
                        work[i + j] = (work[i + j] + masXr[j]) % 2;
                    }
                }
            }

            return work; // остаток будет в последних m-1 позициях (в нашем коде m == r+1)
        }

        //Сложение массивов по модулю 2 с опр. позиции (как раньше)
        public static int[] AddingMasMod2(int[] mas1, int[] mas2, int pos)
        {
            int end = pos + mas2.Length;

            for (int i = pos; i < end; i++)
            {
                mas1[i] = (mas1[i] + mas2[i - pos]) % 2;
            }
            return mas1;
        }

        //Смещение на массива r 
        public static int[] Shift(int[] shiftMas, int[] mas, int r)
        {
            for (int i = 0; i < mas.Length; i++)
            {
                shiftMas[i] = mas[i];
            }
            // остальные позиции остаются теми, что были (обычно 0)
            return shiftMas;
        }

        //Преобразование сторки в массив
        public static int[] StrInMas(int[] mas, string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '1')
                    mas[i] = 1;
                else mas[i] = 0;
            }
            return mas;
        }

        //Создание Порождающей матрицы 
        static int[,] CreateGenerationMatrix(int[,] generationMatrix, int[] mas, int k, int n)
        {
            //Заполняем первую строку
            for (int i = 0; i < n; i++)
            {
                if (i < mas.Length)
                {
                    generationMatrix[0, i] = mas[i];
                }
                else
                {
                    generationMatrix[0, i] = 0;
                }
            }

            //Сдвигаем строки вправо
            for (int i = 1; i < k; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    generationMatrix[i, j + 1] = generationMatrix[i - 1, j];
                }
                generationMatrix[i, 0] = generationMatrix[i - 1, n - 1];
            }

            return generationMatrix;
        }

        //Приведение порождающей матрицы к каноническому виду
        static int[,] CreateCanonicalMatrix(int[,] generationMatrix, int k, int n)
        {
            for (int i = 0; i < k; i++)
            {
                int i2 = i + 1;
                for (int j = i + 1; j < k; j++)
                {
                    if (generationMatrix[i, j] == 1)
                    {
                        for (; i2 < k; i2++)
                        {
                            bool repeat = false;
                            if (generationMatrix[i2, j] == 1)
                            {
                                for (int j2 = j - 1; j2 > 0; j2--)
                                {
                                    if (generationMatrix[i2, j2] == 1)
                                    {
                                        repeat = true;
                                    }
                                }
                                if (repeat)
                                    continue;
                                Console.WriteLine(i + " " + i2);
                                AddingLinesMatrixMod2(generationMatrix, i, i2, n);
                                i2++;
                                break;
                            }
                        }
                    }
                }
            }

            return generationMatrix;
        }

        //Преобразование канонической матрицы в проверочную
        static int[,] CreateCheckMatrix(int[,] checkMatrix, int[,] generationMatrix, int k, int n)
        {
            int r = n - k;

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < r; j++)
                {
                    checkMatrix[i, j] = generationMatrix[i, k + j];
                }
            }

            for (int i = k; i < n; i++)
            {
                for (int j = 0; j < r; j++)
                {
                    if (j == i - k)
                    {
                        checkMatrix[i, j] = 1;
                    }
                    else
                    {
                        checkMatrix[i, j] = 0;
                    }
                }
            }

            return checkMatrix;
        }

        //Сложение строк матрицы
        public static int[,] AddingLinesMatrixMod2(int[,] matrix, int str1, int str2, int lengthString)
        {
            for (int i = 0; i < lengthString; i++)
            {
                matrix[str1, i] = (matrix[str1, i] + matrix[str2, i]) % 2;
            }
            return matrix;
        }

        //вывод матрицы
        public static void OutMatrix(int[,] matrix, int k, int n)
        {
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(matrix[i, j]);
                }
                Console.WriteLine();
            }
        }

        //вывод одномерного массива + перевод строки
        public static void OutMass(int[] mas)
        {
            for (int i = 0; i < mas.Length; i++)
            {
                Console.Write(mas[i]);
            }
            Console.WriteLine("\n");
        }

        // вывод одномерного массива в одной строке (без перевода строки после)
        public static void OutMassInline(int[] mas)
        {
            for (int i = 0; i < mas.Length; i++)
            {
                Console.Write(mas[i]);
            }
            Console.Write("  ");
        }
    }
}
