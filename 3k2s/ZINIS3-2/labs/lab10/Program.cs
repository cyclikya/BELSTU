using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Lab10DigitalSignature
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Лабораторная работа №10. Электронная цифровая подпись");
                Console.WriteLine("1 - RSA");
                Console.WriteLine("2 - Эль-Гамаль");
                Console.WriteLine("3 - Шнорр");
                Console.WriteLine("4 - Запустить все алгоритмы");
                Console.WriteLine("0 - Выход");
                Console.Write("Выберите пункт: ");

                string? choice = Console.ReadLine();
                if (choice == "0") return;

                Console.WriteLine();
                string message = ReadMessage();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": DemoRsa(message); break;
                    case "2": DemoElGamal(message); break;
                    case "3": DemoSchnorr(message); break;
                    case "4":
                        DemoRsa(message);
                        DemoElGamal(message);
                        DemoSchnorr(message);
                        break;
                    default:
                        Console.WriteLine("Неизвестный пункт меню.");
                        break;
                }

                Console.WriteLine("\nНажмите Enter, чтобы вернуться в меню...");
                Console.ReadLine();
            }
        }

        static string ReadMessage()
        {
            Console.Write("Введите сообщение или оставьте пустым для примера: ");
            string? text = Console.ReadLine();
            return string.IsNullOrWhiteSpace(text)
                ? "Лабораторная работа 10. Проверка электронной цифровой подписи."
                : text;
        }

        static void DemoRsa(string message)
        {
            Console.WriteLine("====== RSA ======");
            var sw = Stopwatch.StartNew();
            RsaKeys keys = RsaSignature.GenerateKeys(512);
            sw.Stop();
            Console.WriteLine($"Генерация ключей: {sw.Elapsed.TotalMilliseconds:F3} мс");
            Console.WriteLine($"Открытый ключ: e = {keys.E}, n = {Short(keys.N)}");
            Console.WriteLine($"Закрытый ключ: d = {Short(keys.D)}, n = {Short(keys.N)}");

            sw.Restart();
            BigInteger signature = RsaSignature.Sign(message, keys);
            sw.Stop();
            Console.WriteLine($"Подпись S = H(M)^d mod n: {Short(signature)}");
            Console.WriteLine($"Время подписи: {sw.Elapsed.TotalMilliseconds:F3} мс");

            sw.Restart();
            bool ok = RsaSignature.Verify(message, signature, keys);
            sw.Stop();
            Console.WriteLine($"Проверка исходного сообщения: {ok}");
            Console.WriteLine($"Время проверки: {sw.Elapsed.TotalMilliseconds:F3} мс");

            bool bad = RsaSignature.Verify(message + "!", signature, keys);
            Console.WriteLine($"Проверка измененного сообщения: {bad}");
            Console.WriteLine();
        }

        static void DemoElGamal(string message)
        {
            Console.WriteLine("====== Эль-Гамаль ======");
            var sw = Stopwatch.StartNew();
            ElGamalKeys keys = ElGamalSignature.GenerateKeys(256);
            sw.Stop();
            Console.WriteLine($"Генерация ключей: {sw.Elapsed.TotalMilliseconds:F3} мс");
            Console.WriteLine($"Открытый ключ: p = {Short(keys.P)}, g = {keys.G}, y = {Short(keys.Y)}");
            Console.WriteLine($"Закрытый ключ: x = {Short(keys.X)}");

            sw.Restart();
            ElGamalSignatureValue signature = ElGamalSignature.Sign(message, keys);
            sw.Stop();
            Console.WriteLine($"Подпись: r = {Short(signature.R)}, s = {Short(signature.S)}");
            Console.WriteLine($"Время подписи: {sw.Elapsed.TotalMilliseconds:F3} мс");

            sw.Restart();
            bool ok = ElGamalSignature.Verify(message, signature, keys);
            sw.Stop();
            Console.WriteLine($"Проверка исходного сообщения: {ok}");
            Console.WriteLine($"Время проверки: {sw.Elapsed.TotalMilliseconds:F3} мс");

            bool bad = ElGamalSignature.Verify(message + "!", signature, keys);
            Console.WriteLine($"Проверка измененного сообщения: {bad}");
            Console.WriteLine();
        }

        static void DemoSchnorr(string message)
        {
            Console.WriteLine("====== Шнорр ======");
             var keys = SchnorrSignature.GenerateDemoKeys();
            Console.WriteLine($"Параметры: p = {keys.P}, q = {keys.Q}, g = {keys.G}");
            Console.WriteLine($"Открытый ключ: y = {keys.Y}");
            Console.WriteLine($"Закрытый ключ: x = {keys.X}");

            var sw = Stopwatch.StartNew();
            SchnorrSignatureValue signature = SchnorrSignature.Sign(message, keys);
            sw.Stop();
            Console.WriteLine($"Подпись: e = {signature.E}, s = {signature.S}");
            Console.WriteLine($"Время подписи: {sw.Elapsed.TotalMilliseconds:F3} мс");

            sw.Restart();
            bool ok = SchnorrSignature.Verify(message, signature, keys);
            sw.Stop();
            Console.WriteLine($"Проверка исходного сообщения: {ok}");
            Console.WriteLine($"Время проверки: {sw.Elapsed.TotalMilliseconds:F3} мс");

            bool bad = SchnorrSignature.Verify(message + "!", signature, keys);
            Console.WriteLine($"Проверка измененного сообщения: {bad}");
            Console.WriteLine();
        }

        static string Short(BigInteger value)
        {
            string s = value.ToString();
            return s.Length <= 60 ? s : s.Substring(0, 30) + "..." + s.Substring(s.Length - 20);
        }
    }

    public record RsaKeys(BigInteger N, BigInteger E, BigInteger D);

    public static class RsaSignature
    {
        public static RsaKeys GenerateKeys(int bits)
        {
            BigInteger e = 65537;
            BigInteger p, q, n, phi;
            do
            {
                p = CryptoMath.GeneratePrime(bits / 2);
                q = CryptoMath.GeneratePrime(bits / 2);
                n = p * q;
                phi = (p - 1) * (q - 1);
            } while (p == q || BigInteger.GreatestCommonDivisor(e, phi) != 1);

            BigInteger d = CryptoMath.ModInverse(e, phi);
            return new RsaKeys(n, e, d);
        }

        public static BigInteger Sign(string message, RsaKeys keys)
        {
            BigInteger h = CryptoMath.HashToBigInteger(message) % keys.N;
            return BigInteger.ModPow(h, keys.D, keys.N);
        }

        public static bool Verify(string message, BigInteger signature, RsaKeys keys)
        {
            BigInteger expectedHash = CryptoMath.HashToBigInteger(message) % keys.N;
            BigInteger actualHash = BigInteger.ModPow(signature, keys.E, keys.N);
            return expectedHash == actualHash;
        }
    }

    public record ElGamalKeys(BigInteger P, BigInteger G, BigInteger X, BigInteger Y);
    public record ElGamalSignatureValue(BigInteger R, BigInteger S);

    public static class ElGamalSignature
    {
        public static ElGamalKeys GenerateKeys(int bits)
        {
            BigInteger p = CryptoMath.GeneratePrime(bits);
            BigInteger g = 2;
            BigInteger x = CryptoMath.RandomBetween(2, p - 2);
            BigInteger y = BigInteger.ModPow(g, x, p);
            return new ElGamalKeys(p, g, x, y);
        }

        public static ElGamalSignatureValue Sign(string message, ElGamalKeys keys)
        {
            BigInteger h = CryptoMath.Mod(CryptoMath.HashToBigInteger(message), keys.P - 1);
            BigInteger k;
            do
            {
                k = CryptoMath.RandomBetween(2, keys.P - 2);
            } while (BigInteger.GreatestCommonDivisor(k, keys.P - 1) != 1);

            BigInteger r = BigInteger.ModPow(keys.G, k, keys.P);
            BigInteger kInv = CryptoMath.ModInverse(k, keys.P - 1);
            BigInteger s = CryptoMath.Mod((h - keys.X * r) * kInv, keys.P - 1);
            return new ElGamalSignatureValue(r, s);
        }

        public static bool Verify(string message, ElGamalSignatureValue signature, ElGamalKeys keys)
        {
            if (signature.R <= 0 || signature.R >= keys.P) return false;
            BigInteger h = CryptoMath.Mod(CryptoMath.HashToBigInteger(message), keys.P - 1);
            BigInteger left = BigInteger.ModPow(keys.G, h, keys.P);
            BigInteger right = CryptoMath.Mod(
                BigInteger.ModPow(keys.Y, signature.R, keys.P) * BigInteger.ModPow(signature.R, signature.S, keys.P),
                keys.P);
            return left == right;
        }
    }

    public record SchnorrKeys(BigInteger P, BigInteger Q, BigInteger G, BigInteger X, BigInteger Y);
    public record SchnorrSignatureValue(BigInteger E, BigInteger S);

    public static class SchnorrSignature
    {
        public static SchnorrKeys GenerateDemoKeys()
        {
            BigInteger p = 23;
            BigInteger q = 11;
            BigInteger g = 4;
            BigInteger x = 3;
            BigInteger y = BigInteger.ModPow(g, x, p);
            return new SchnorrKeys(p, q, g, x, y);
        }

        public static SchnorrSignatureValue Sign(string message, SchnorrKeys keys)
        {
            BigInteger k = CryptoMath.RandomBetween(1, keys.Q - 1);
            BigInteger r = BigInteger.ModPow(keys.G, k, keys.P);
            BigInteger e = HashMessageWithNumber(message, r) % keys.Q;
            BigInteger s = CryptoMath.Mod(k - keys.X * e, keys.Q);
            return new SchnorrSignatureValue(e, s);
        }

        public static bool Verify(string message, SchnorrSignatureValue signature, SchnorrKeys keys)
        {
            if (signature.E < 0 || signature.E >= keys.Q) return false;
            if (signature.S < 0 || signature.S >= keys.Q) return false;

            BigInteger r = CryptoMath.Mod(
                BigInteger.ModPow(keys.G, signature.S, keys.P) * BigInteger.ModPow(keys.Y, signature.E, keys.P),
                keys.P);
            BigInteger expectedE = HashMessageWithNumber(message, r) % keys.Q;
            return expectedE == signature.E;
        }

        static BigInteger HashMessageWithNumber(string message, BigInteger number)
        {
            return CryptoMath.HashToBigInteger(message + "|" + number);
        }
    }

    public static class CryptoMath
    {
        public static BigInteger HashToBigInteger(string message)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(message));
            Array.Reverse(hash);
            byte[] positive = hash.Concat(new byte[] { 0 }).ToArray();
            return new BigInteger(positive);
        }

        public static BigInteger GeneratePrime(int bits)
        {
            while (true)
            {
                BigInteger candidate = RandomBigInteger(bits);
                candidate |= 1;
                if (IsProbablePrime(candidate, 20)) return candidate;
            }
        }

        public static BigInteger RandomBigInteger(int bits)
        {
            int bytesCount = (bits + 7) / 8;
            byte[] bytes = new byte[bytesCount + 1];
            RandomNumberGenerator.Fill(bytes.AsSpan(0, bytesCount));
            bytes[bytesCount - 1] |= 0x40;
            bytes[bytesCount] = 0;
            return new BigInteger(bytes);
        }

        public static BigInteger RandomBetween(BigInteger min, BigInteger max)
        {
            if (min > max) throw new ArgumentException("min > max");
            BigInteger range = max - min + 1;
            byte[] bytes = range.ToByteArray(isUnsigned: true, isBigEndian: false);

            while (true)
            {
                RandomNumberGenerator.Fill(bytes);
                BigInteger value = new BigInteger(bytes, isUnsigned: true, isBigEndian: false);
                if (value < range) return min + value;
            }
        }

        public static bool IsProbablePrime(BigInteger value, int rounds)
        {
            if (value < 2) return false;
            if (value == 2 || value == 3) return true;
            if (value % 2 == 0) return false;

            BigInteger d = value - 1;
            int s = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            for (int i = 0; i < rounds; i++)
            {
                BigInteger a = RandomBetween(2, value - 2);
                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1) continue;

                bool passed = false;
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == value - 1)
                    {
                        passed = true;
                        break;
                    }
                }

                if (!passed) return false;
            }

            return true;
        }

        public static BigInteger ModInverse(BigInteger a, BigInteger mod)
        {
            BigInteger t = 0;
            BigInteger newT = 1;
            BigInteger r = mod;
            BigInteger newR = Mod(a, mod);

            while (newR != 0)
            {
                BigInteger quotient = r / newR;
                (t, newT) = (newT, t - quotient * newT);
                (r, newR) = (newR, r - quotient * newR);
            }

            if (r > 1) throw new ArgumentException("Обратного элемента не существует.");
            if (t < 0) t += mod;
            return t;
        }

        public static BigInteger Mod(BigInteger value, BigInteger mod)
        {
            BigInteger result = value % mod;
            return result < 0 ? result + mod : result;
        }
    }
}
