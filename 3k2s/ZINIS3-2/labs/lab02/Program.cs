using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab2_SubstitutionCiphers
{
    class Program
    {
        static readonly string Alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        const string Keyword = "БЕЗОПАСНОСТЬ";

        //C:\General\BELSTU\3k2s\ZINIS3-2\labs\lab02\input.txt

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("Введите путь к текстовому файлу:");
            string path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            string text = File.ReadAllText(path, Encoding.UTF8).ToUpper();

            Console.WriteLine("\nВыберите метод:");
            Console.WriteLine("1 - Цезарь с ключевым словом");
            Console.WriteLine("2 - Таблица Трисемуса");
            string choice = Console.ReadLine();

            Stopwatch se = new Stopwatch();
            Stopwatch sd = new Stopwatch();

            string encrypted = "";
            string decrypted = "";

            if (choice == "1")
            {
                se.Start();
                string cipherAlphabet = BuildCipherAlphabet();
                encrypted = CaesarEncrypt(text, cipherAlphabet);
                decrypted = CaesarDecrypt(encrypted, cipherAlphabet);
                se.Stop();
            }
            else
            {
                sd.Start();
                string keyAlphabet = BuildCipherAlphabet();
                encrypted = TrithemiusEncrypt(text, keyAlphabet);
                decrypted = TrithemiusDecrypt(encrypted, keyAlphabet);
                sd.Stop();

            }


            Console.WriteLine($"\nВремя шифрования: {se.ElapsedMilliseconds} мс");
            Console.WriteLine($"\nВремя расшифрования: {sd.ElapsedMilliseconds} мс");

            File.WriteAllText("encrypted.txt", encrypted, Encoding.UTF8);
            File.WriteAllText("decrypted.txt", decrypted, Encoding.UTF8);

            Console.WriteLine("\n--- Статистика исходного текста ---");
            PrintHistogram(text);

            Console.WriteLine("\n--- Статистика зашифрованного текста ---");
            PrintHistogram(encrypted);

            Console.WriteLine("\nГотово.");
        }

        // Построение ключевого алфавита
        static string BuildCipherAlphabet()
        {
            string uniqueKey = new string(Keyword
                .Where(c => Alphabet.Contains(c))
                .Distinct()
                .ToArray());

            string rest = new string(Alphabet
                .Where(c => !uniqueKey.Contains(c))
                .ToArray());

            return uniqueKey + rest;
        }

        // Цезарь с ключевым словом
        static string CaesarEncrypt(string text, string cipherAlphabet)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                int index = Alphabet.IndexOf(c);
                if (index >= 0)
                    sb.Append(cipherAlphabet[index]);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        static string CaesarDecrypt(string text, string cipherAlphabet)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                int index = cipherAlphabet.IndexOf(c);
                if (index >= 0)
                    sb.Append(Alphabet[index]);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        // =========================
        // Таблица Трисемуса (правильная версия)
        // =========================
        static string TrithemiusEncrypt(string text, string keyAlphabet)
        {
            int columns = 6;
            int length = keyAlphabet.Length;

            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                int index = keyAlphabet.IndexOf(c);

                if (index >= 0)
                {
                    int newIndex = index + columns;

                    if (newIndex >= length)
                        newIndex = newIndex % columns;

                    sb.Append(keyAlphabet[newIndex]);
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        static string TrithemiusDecrypt(string text, string keyAlphabet)
        {
            int columns = 6;
            int length = keyAlphabet.Length;

            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                int index = keyAlphabet.IndexOf(c);

                if (index >= 0)
                {
                    int newIndex = index - columns;

                    if (newIndex < 0)
                    {
                        int remainder = index % columns;
                        int lastRowIndex = remainder;

                        while (lastRowIndex + columns < length)
                            lastRowIndex += columns;

                        newIndex = lastRowIndex;
                    }

                    sb.Append(keyAlphabet[newIndex]);
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        // =========================
        // Гистограмма
        // =========================
        static void PrintHistogram(string text)
        {
            var freq = new Dictionary<char, int>();

            foreach (char c in Alphabet)
                freq[c] = 0;

            foreach (char c in text)
                if (freq.ContainsKey(c))
                    freq[c]++;

            int total = freq.Values.Sum();

            foreach (var pair in freq.OrderByDescending(p => p.Value))
            {
                double percent = total > 0 ? (double)pair.Value / total * 100 : 0;
                Console.WriteLine($"{pair.Key} : {pair.Value} ({percent:F2}%)");
            }
        }
    }
}