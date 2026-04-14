using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Lab8_Console
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("Лабораторная работа №8");
                Console.WriteLine("1 - Задание 1: исследование времени вычисления y = a^x mod n");
                Console.WriteLine("2 - Задание 2: RSA и Эль-Гамаль");
                Console.WriteLine("0 - Выход");

                int task = ReadInt("Выберите задание: ");

                if (task == 0)
                    break;

                if (task != 1 && task != 2)
                {
                    Console.WriteLine("Некорректный выбор.");
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine("1 - Ручной ввод");
                Console.WriteLine("2 - Автоматическая генерация");
                int mode = ReadInt("Выберите режим: ");

                if (mode != 1 && mode != 2)
                {
                    Console.WriteLine("Некорректный выбор режима.");
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine();

                try
                {
                    if (task == 1)
                        RunTask1(mode);
                    else
                        RunTask2(mode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("Нажмите Enter для продолжения...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        // ============================
        // ЗАДАНИЕ 1
        // ============================
        static void RunTask1(int mode)
        {
            BigInteger a;
            BigInteger n;
            List<BigInteger> xValues;

            if (mode == 1)
            {
                a = ReadBigInteger("Введите a: ");
                n = ReadBigInteger("Введите n: ");

                int count = ReadInt("Введите количество значений x: ");
                xValues = new List<BigInteger>();

                for (int i = 0; i < count; i++)
                {
                    xValues.Add(ReadBigInteger($"Введите x[{i + 1}]: "));
                }
            }
            else
            {
                RandomNumberGenerator rng = RandomNumberGenerator.Create();

                a = RandomBigIntegerInRange(5, 35, rng);

                int bitLengthChoice = RandomBigIntegerInRange(0, 1, rng) == 0 ? 1024 : 2048;
                n = GenerateRandomOddBigInteger(bitLengthChoice, rng);

                xValues = GeneratePrimeLikeXValues();

                Console.WriteLine("Автоматически сгенерированные данные:");
                Console.WriteLine($"a = {a}");
                Console.WriteLine($"n bit length ≈ {GetBitLength(n)}");
                Console.WriteLine("x:");
                foreach (var x in xValues)
                    Console.WriteLine(x);
                Console.WriteLine();
            }

            Console.WriteLine("Таблица результатов:");
            Console.WriteLine("---------------------------------------------------------------------------------------------------");
            Console.WriteLine($"| {"№",3} | {"x",30} | {"y = a^x mod n",35} | {"ticks",12} | {"ms",12} |");
            Console.WriteLine("---------------------------------------------------------------------------------------------------");

            int index = 1;
            foreach (var x in xValues)
            {
                Stopwatch sw = Stopwatch.StartNew();
                BigInteger y = ModPowBig(a, x, n);
                sw.Stop();

                Console.WriteLine(
                    $"| {index,3} | {Truncate(x.ToString(), 30),30} | {Truncate(y.ToString(), 35),35} | {sw.ElapsedTicks,12} | {sw.Elapsed.TotalMilliseconds,12:F6} |"
                );

                index++;
            }

            Console.WriteLine("---------------------------------------------------------------------------------------------------");
        }

        static List<BigInteger> GeneratePrimeLikeXValues()
        {
            return new List<BigInteger>
            {
                1009,
                10007,
                100003,
                1000003,
                BigInteger.Parse("10000019"),
                BigInteger.Parse("100000007"),
                BigInteger.Parse("1000000007"),
                BigInteger.Parse("10000000019")
            };
        }

        static BigInteger ModPowBig(BigInteger a, BigInteger x, BigInteger n)
        {
            if (n == 1)
                return 0;

            BigInteger result = 1;
            BigInteger baseValue = ((a % n) + n) % n;
            BigInteger exponent = x;

            while (exponent > 0)
            {
                if (!exponent.IsEven)
                    result = (result * baseValue) % n;

                baseValue = (baseValue * baseValue) % n;
                exponent >>= 1;
            }

            return result;
        }

        // ============================
        // ЗАДАНИЕ 2
        // ============================
        static void RunTask2(int mode)
        {
            string sourceText;

            if (mode == 1)
            {
                Console.Write("Введите текст для шифрования: ");
                sourceText = Console.ReadLine() ?? string.Empty;
            }
            else
            {
                sourceText = "Ugorenko Violetta Romanovna";
                Console.WriteLine($"Автоматически выбран текст: {sourceText}");
            }

            Console.WriteLine();
            Console.WriteLine("ASCII представление:");
            byte[] asciiBytes = Encoding.ASCII.GetBytes(TransliterateToAscii(sourceText));
            Console.WriteLine(string.Join(" ", asciiBytes));

            Console.WriteLine();
            Console.WriteLine("Base64 представление:");
            string base64Source = Convert.ToBase64String(asciiBytes);
            Console.WriteLine(base64Source);

            Console.WriteLine();
            Console.WriteLine("===== RSA =====");
            RunRsaDemo(sourceText);

            Console.WriteLine();
            Console.WriteLine("===== Эль-Гамаль =====");
            RunElGamalDemo(sourceText, mode);
        }

        static void RunRsaDemo(string sourceText)
        {
            string asciiText = TransliterateToAscii(sourceText);
            byte[] data = Encoding.ASCII.GetBytes(asciiText);

            using RSA rsa = RSA.Create(2048);

            Stopwatch swEncrypt = Stopwatch.StartNew();
            byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            swEncrypt.Stop();

            string encryptedBase64 = Convert.ToBase64String(encrypted);

            Stopwatch swDecrypt = Stopwatch.StartNew();
            byte[] decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);
            swDecrypt.Stop();

            string decryptedText = Encoding.ASCII.GetString(decrypted);

            Console.WriteLine("Исходный текст:");
            Console.WriteLine(asciiText);

            Console.WriteLine();
            Console.WriteLine("Зашифрованный текст RSA в Base64:");
            Console.WriteLine(encryptedBase64);

            Console.WriteLine();
            Console.WriteLine("Расшифрованный текст RSA:");
            Console.WriteLine(decryptedText);

            Console.WriteLine();
            Console.WriteLine($"Время шифрования RSA: {swEncrypt.Elapsed.TotalMilliseconds:F6} ms");
            Console.WriteLine($"Время расшифрования RSA: {swDecrypt.Elapsed.TotalMilliseconds:F6} ms");
        }

        static void RunElGamalDemo(string sourceText, int mode)
        {
            string asciiText = TransliterateToAscii(sourceText);
            byte[] data = Encoding.ASCII.GetBytes(asciiText);

            ElGamalKeys keys = mode == 1
                ? ReadElGamalKeysManual()
                : GenerateElGamalKeysAuto();

            Console.WriteLine($"p = {keys.P}");
            Console.WriteLine($"g = {keys.G}");
            Console.WriteLine($"x = {keys.X}");
            Console.WriteLine($"y = {keys.Y}");

            Stopwatch swEncrypt = Stopwatch.StartNew();
            List<ElGamalCipherPair> encrypted = ElGamalEncryptBytes(data, keys);
            swEncrypt.Stop();

            string serialized = SerializeElGamalCipher(encrypted);
            string base64Cipher = Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized));

            Stopwatch swDecrypt = Stopwatch.StartNew();
            byte[] decrypted = ElGamalDecryptBytes(encrypted, keys);
            swDecrypt.Stop();

            string decryptedText = Encoding.ASCII.GetString(decrypted);

            Console.WriteLine();
            Console.WriteLine("Зашифрованный текст Эль-Гамаля:");
            Console.WriteLine(serialized);

            Console.WriteLine();
            Console.WriteLine("Зашифрованный текст Эль-Гамаля в Base64:");
            Console.WriteLine(base64Cipher);

            Console.WriteLine();
            Console.WriteLine("Расшифрованный текст Эль-Гамаля:");
            Console.WriteLine(decryptedText);

            Console.WriteLine();
            Console.WriteLine($"Время шифрования Эль-Гамаля: {swEncrypt.Elapsed.TotalMilliseconds:F6} ms");
            Console.WriteLine($"Время расшифрования Эль-Гамаля: {swDecrypt.Elapsed.TotalMilliseconds:F6} ms");
        }

        static ElGamalKeys ReadElGamalKeysManual()
        {
            Console.WriteLine("Введите параметры Эль-Гамаля.");
            Console.WriteLine("Важно: p должно быть простым и больше 255.");

            BigInteger p = ReadBigInteger("Введите p: ");
            BigInteger g = ReadBigInteger("Введите g: ");
            BigInteger x = ReadBigInteger("Введите секретный ключ x: ");

            if (p <= 255)
                throw new Exception("p должно быть больше 255.");

            BigInteger y = ModPowBig(g, x, p);

            return new ElGamalKeys
            {
                P = p,
                G = g,
                X = x,
                Y = y
            };
        }

        static ElGamalKeys GenerateElGamalKeysAuto()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            BigInteger p = GeneratePrimeInRange(50000, 100000, rng);
            BigInteger g = FindPrimitiveRoot(p);
            BigInteger x = RandomBigIntegerInRange(2, p - 2, rng);
            BigInteger y = ModPowBig(g, x, p);

            return new ElGamalKeys
            {
                P = p,
                G = g,
                X = x,
                Y = y
            };
        }

        static List<ElGamalCipherPair> ElGamalEncryptBytes(byte[] data, ElGamalKeys keys)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            List<ElGamalCipherPair> result = new List<ElGamalCipherPair>();

            foreach (byte b in data)
            {
                BigInteger m = b;
                BigInteger k = RandomBigIntegerInRange(2, keys.P - 2, rng);

                BigInteger a = ModPowBig(keys.G, k, keys.P);
                BigInteger bPart = (ModPowBig(keys.Y, k, keys.P) * m) % keys.P;

                result.Add(new ElGamalCipherPair
                {
                    A = a,
                    B = bPart
                });
            }

            return result;
        }

        static byte[] ElGamalDecryptBytes(List<ElGamalCipherPair> cipher, ElGamalKeys keys)
        {
            List<byte> bytes = new List<byte>();

            foreach (var pair in cipher)
            {
                BigInteger ax = ModPowBig(pair.A, keys.X, keys.P);
                BigInteger inverse = ModInverse(ax, keys.P);
                BigInteger m = (pair.B * inverse) % keys.P;

                if (m < 0 || m > 255)
                    throw new Exception("Ошибка расшифрования Эль-Гамаля: получен байт вне диапазона.");

                bytes.Add((byte)m);
            }

            return bytes.ToArray();
        }

        static string SerializeElGamalCipher(List<ElGamalCipherPair> cipher)
        {
            return string.Join(";", cipher.Select(c => $"{c.A},{c.B}"));
        }

        // ============================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ============================
        static string TransliterateToAscii(string text)
        {
            Dictionary<char, string> map = new Dictionary<char, string>
            {
                ['А'] = "A",
                ['а'] = "a",
                ['Б'] = "B",
                ['б'] = "b",
                ['В'] = "V",
                ['в'] = "v",
                ['Г'] = "G",
                ['г'] = "g",
                ['Д'] = "D",
                ['д'] = "d",
                ['Е'] = "E",
                ['е'] = "e",
                ['Ё'] = "E",
                ['ё'] = "e",
                ['Ж'] = "Zh",
                ['ж'] = "zh",
                ['З'] = "Z",
                ['з'] = "z",
                ['И'] = "I",
                ['и'] = "i",
                ['Й'] = "Y",
                ['й'] = "y",
                ['К'] = "K",
                ['к'] = "k",
                ['Л'] = "L",
                ['л'] = "l",
                ['М'] = "M",
                ['м'] = "m",
                ['Н'] = "N",
                ['н'] = "n",
                ['О'] = "O",
                ['о'] = "o",
                ['П'] = "P",
                ['п'] = "p",
                ['Р'] = "R",
                ['р'] = "r",
                ['С'] = "S",
                ['с'] = "s",
                ['Т'] = "T",
                ['т'] = "t",
                ['У'] = "U",
                ['у'] = "u",
                ['Ф'] = "F",
                ['ф'] = "f",
                ['Х'] = "Kh",
                ['х'] = "kh",
                ['Ц'] = "Ts",
                ['ц'] = "ts",
                ['Ч'] = "Ch",
                ['ч'] = "ch",
                ['Ш'] = "Sh",
                ['ш'] = "sh",
                ['Щ'] = "Sch",
                ['щ'] = "sch",
                ['Ъ'] = "",
                ['ъ'] = "",
                ['Ы'] = "Y",
                ['ы'] = "y",
                ['Ь'] = "",
                ['ь'] = "",
                ['Э'] = "E",
                ['э'] = "e",
                ['Ю'] = "Yu",
                ['ю'] = "yu",
                ['Я'] = "Ya",
                ['я'] = "ya"
            };

            StringBuilder sb = new StringBuilder();
            foreach (char ch in text)
            {
                if (map.ContainsKey(ch))
                    sb.Append(map[ch]);
                else if (ch <= 127)
                    sb.Append(ch);
                else
                    sb.Append('?');
            }

            return sb.ToString();
        }

        static BigInteger ReadBigInteger(string msg)
        {
            while (true)
            {
                Console.Write(msg);
                string? input = Console.ReadLine();

                if (BigInteger.TryParse(input, out BigInteger value))
                    return value;

                Console.WriteLine("Ошибка ввода. Повторите.");
            }
        }

        static int ReadInt(string msg)
        {
            while (true)
            {
                Console.Write(msg);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                    return value;

                Console.WriteLine("Ошибка ввода. Повторите.");
            }
        }

        static string Truncate(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength - 3) + "...";
        }

        static BigInteger RandomBigIntegerInRange(BigInteger min, BigInteger max, RandomNumberGenerator rng)
        {
            if (min > max)
                throw new ArgumentException("min > max");

            BigInteger range = max - min + 1;
            int bytesLength = range.ToByteArray().Length;
            byte[] bytes = new byte[bytesLength];

            BigInteger result;
            do
            {
                rng.GetBytes(bytes);
                bytes[^1] &= 0x7F;
                result = new BigInteger(bytes);
            } while (result >= range || result < 0);

            return min + result;
        }

        static BigInteger GenerateRandomOddBigInteger(int bitLength, RandomNumberGenerator rng)
        {
            int byteLength = (bitLength + 7) / 8;
            byte[] bytes = new byte[byteLength];
            rng.GetBytes(bytes);

            int highestBitIndex = (bitLength - 1) % 8;
            bytes[^1] |= (byte)(1 << highestBitIndex);
            bytes[0] |= 1;

            byte[] extended = new byte[byteLength + 1];
            Array.Copy(bytes, extended, byteLength);

            return new BigInteger(extended);
        }

        static int GetBitLength(BigInteger value)
        {
            byte[] bytes = value.ToByteArray();
            int msb = bytes[^1];
            int bits = (bytes.Length - 1) * 8;

            while (msb > 0)
            {
                bits++;
                msb >>= 1;
            }

            return bits;
        }

        static BigInteger GeneratePrimeInRange(int min, int max, RandomNumberGenerator rng)
        {
            while (true)
            {
                BigInteger candidate = RandomBigIntegerInRange(min, max, rng);
                if (candidate.IsEven)
                    candidate++;

                if (IsPrime(candidate))
                    return candidate;
            }
        }

        static bool IsPrime(BigInteger n)
        {
            if (n < 2)
                return false;
            if (n == 2 || n == 3)
                return true;
            if (n % 2 == 0)
                return false;

            for (BigInteger i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0)
                    return false;
            }

            return true;
        }

        static BigInteger FindPrimitiveRoot(BigInteger p)
        {
            BigInteger phi = p - 1;
            List<BigInteger> factors = PrimeFactors(phi);

            for (BigInteger g = 2; g < p; g++)
            {
                bool ok = true;
                foreach (BigInteger factor in factors)
                {
                    if (ModPowBig(g, phi / factor, p) == 1)
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                    return g;
            }

            throw new Exception("Первообразный корень не найден.");
        }

        static List<BigInteger> PrimeFactors(BigInteger n)
        {
            List<BigInteger> factors = new List<BigInteger>();

            while (n % 2 == 0)
            {
                factors.Add(2);
                while (n % 2 == 0)
                    n /= 2;
            }

            for (BigInteger i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0)
                {
                    factors.Add(i);
                    while (n % i == 0)
                        n /= i;
                }
            }

            if (n > 2)
                factors.Add(n);

            return factors.Distinct().ToList();
        }

        static BigInteger ModInverse(BigInteger a, BigInteger mod)
        {
            BigInteger t = 0, newT = 1;
            BigInteger r = mod, newR = a % mod;

            while (newR != 0)
            {
                BigInteger q = r / newR;

                (t, newT) = (newT, t - q * newT);
                (r, newR) = (newR, r - q * newR);
            }

            if (r > 1)
                throw new Exception("Обратный элемент не существует.");

            if (t < 0)
                t += mod;

            return t;
        }
    }

    internal class ElGamalKeys
    {
        public BigInteger P { get; set; }
        public BigInteger G { get; set; }
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }
    }

    internal class ElGamalCipherPair
    {
        public BigInteger A { get; set; }
        public BigInteger B { get; set; }
    }
}