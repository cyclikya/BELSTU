using System;
using System.Collections.Generic;

namespace Lab1_Variant12
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Вычислить НОД двух чисел");
                Console.WriteLine("2. Вычислить НОД трех чисел");
                Console.WriteLine("3. Найти простые числа в диапазоне");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GCDTwo();
                        break;
                    case "2":
                        GCDThree();
                        break;
                    case "3":
                        FindPrimes();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Pause();
                        break;
                }
            }
        }

        // ================= НОД =================

        static void GCDTwo()
        {
            Console.Write("Введите первое число: ");
            long a = long.Parse(Console.ReadLine());

            Console.Write("Введите второе число: ");
            long b = long.Parse(Console.ReadLine());

            Console.WriteLine($"НОД({a}, {b}) = {GCD(a, b)}");
            Pause();
        }

        static void GCDThree()
        {
            Console.Write("Введите первое число: ");
            long a = long.Parse(Console.ReadLine());

            Console.Write("Введите второе число: ");
            long b = long.Parse(Console.ReadLine());

            Console.Write("Введите третье число: ");
            long c = long.Parse(Console.ReadLine());

            long result = GCD(GCD(a, b), c);

            Console.WriteLine($"НОД({a}, {b}, {c}) = {result}");
            Pause();
        }

        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return Math.Abs(a);
        }

        // ================= ПРОСТЫЕ ЧИСЛА =================

        static void FindPrimes()
        {
            Console.Write("Введите начало диапазона: ");
            int start = int.Parse(Console.ReadLine());

            Console.Write("Введите конец диапазона: ");
            int end = int.Parse(Console.ReadLine());

            List<int> primes = Sieve(start, end);

            Console.WriteLine("\nПростые числа:");
            foreach (var p in primes)
                Console.Write(p + " ");

            Console.WriteLine($"\n\nКоличество: {primes.Count}");
            Pause();
        }

        static List<int> Sieve(int start, int end)
        {
            bool[] isPrime = new bool[end + 1];
            for (int i = 2; i <= end; i++)
                isPrime[i] = true;

            for (int i = 2; i * i <= end; i++)
            {
                if (isPrime[i])
                {
                    for (int j = i * i; j <= end; j += i)
                        isPrime[j] = false;
                }
            }

            List<int> result = new List<int>();

            for (int i = Math.Max(2, start); i <= end; i++)
                if (isPrime[i])
                    result.Add(i);

            return result;
        }

        static void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
