using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace lab07
{
    /// <summary>
    /// Реализация ранцевого шифра Меркла-Хеллмана.
    /// 
    /// Принцип работы:
    /// - Тайный ключ: сверхвозрастающая последовательность d
    /// - Открытый ключ: нормальная последовательность e, полученная из d
    /// - Шифрование: текст → биты → суммы элементов e (укладка ранца)
    /// - Расшифрование: суммы → обратное преобразование → решение лёгкого ранца с d
    /// </summary>
    public class KnapsackCipherEngine
    {
        // === КЛЮЧЕВАЯ ИНФОРМАЦИЯ ===

        /// <summary>
        /// Тайный ключ — сверхвозрастающая последовательность.
        /// Каждый элемент больше суммы всех предыдущих.
        /// </summary>
        public BigInteger[] PrivateKey { get; private set; }

        /// <summary>
        /// Открытый ключ — нормальная последовательность.
        /// Получается из тайного ключа по формуле: e[i] = d[i] * a mod n
        /// </summary>
        public BigInteger[] PublicKey { get; private set; }

        /// <summary>
        /// Модуль n — число, большее суммы всех элементов тайного ключа.
        /// Нужен для модулярной арифметики при создании открытого ключа.
        /// </summary>
        public BigInteger N { get; private set; }

        /// <summary>
        /// Множитель a — число, взаимно простое с n (НОД(a,n) = 1).
        /// Используется для "запутывания" тайного ключа.
        /// </summary>
        public BigInteger A { get; private set; }

        /// <summary>
        /// Обратное к a по модулю n: a * a_inv ≡ 1 (mod n).
        /// Нужно для расшифрования.
        /// </summary>
        public BigInteger AInverse { get; private set; }

        /// <summary>
        /// Размер блока (количество элементов в ранце).
        /// z = 6 для Base64, z = 8 для ASCII, или больше для анализа.
        /// </summary>
        public int Z { get; private set; }

        // Генератор случайных чисел (криптографически стойкий)
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        // =====================================================
        // ГЕНЕРАЦИЯ КЛЮЧЕЙ
        // =====================================================

        /// <summary>
        /// Генерирует пару ключей (тайный и открытый).
        /// 
        /// Алгоритм:
        /// 1. Генерируем сверхвозрастающую последовательность (тайный ключ d)
        /// 2. Выбираем n > суммы всех d[i]
        /// 3. Выбираем a, взаимно простое с n
        /// 4. Вычисляем открытый ключ: e[i] = d[i] * a mod n
        /// 5. Вычисляем a⁻¹ для расшифрования
        /// </summary>
        /// <param name="z">Размер блока (кол-во элементов ранца)</param>
        /// <param name="targetBits">Желаемая битовая длина старшего элемента</param>
        public void GenerateKeys(int z, int targetBits = 100)
        {
            Z = z;

            // Шаг 1: Генерация сверхвозрастающей последовательности
            PrivateKey = GenerateSuperIncreasingSequence(z, targetBits);

            // Шаг 2: Вычисляем сумму всех элементов тайного ключа
            BigInteger sum = BigInteger.Zero;
            for (int i = 0; i < z; i++)
                sum += PrivateKey[i];

            // Шаг 3: Выбираем n > sum (берём случайное число чуть больше суммы)
            // n должно быть строго больше суммы, чтобы модулярная арифметика
            // не «обрезала» значения при создании открытого ключа
            N = GenerateRandomBigInteger(sum.ToByteArray().Length * 8 + 2);
            if (N <= sum)
                N = sum + GenerateRandomBigInteger(32) + 1;

            // Шаг 4: Выбираем a, взаимно простое с n: НОД(a, n) = 1
            // Это необходимое условие существования обратного элемента a⁻¹
            A = GenerateCoprime(N);

            // Шаг 5: Вычисляем обратный элемент a⁻¹ mod n
            // Свойство: a * a⁻¹ ≡ 1 (mod n)
            // Используется расширенный алгоритм Евклида
            AInverse = ModInverse(A, N);

            // Шаг 6: Формируем открытый ключ
            // Каждый элемент: e[i] = d[i] * a mod n
            // Это преобразование «прячет» сверхвозрастающую структуру
            PublicKey = new BigInteger[z];
            for (int i = 0; i < z; i++)
            {
                PublicKey[i] = (PrivateKey[i] * A) % N;
            }
        }

        /// <summary>
        /// Генерирует сверхвозрастающую последовательность.
        /// 
        /// Принцип: каждый следующий элемент = сумма предыдущих + случайное положительное число.
        /// Так гарантируется свойство сверхвозрастания.
        /// 
        /// Последний элемент должен быть ~targetBits бит.
        /// Строим последовательность "снизу вверх":
        /// - Начинаем с маленького числа
        /// - Каждый следующий = сумма всех предыдущих + случайная добавка
        /// </summary>
        private BigInteger[] GenerateSuperIncreasingSequence(int z, int targetBits)
        {
            BigInteger[] seq = new BigInteger[z];

            // Первый элемент — случайное число умеренного размера.
            // Мы хотим, чтобы последний элемент был ~targetBits бит.
            // При каждом шаге сумма примерно удваивается,
            // поэтому первый элемент должен быть примерно targetBits - z бит.
            int firstBits = Math.Max(16, targetBits - z);
            seq[0] = GenerateRandomBigInteger(firstBits);
            if (seq[0] <= 0) seq[0] = 1;

            BigInteger runningSum = seq[0];

            for (int i = 1; i < z; i++)
            {
                // Случайная добавка — чтобы элементы не были предсказуемыми
                BigInteger delta = GenerateRandomBigInteger(
                    Math.Max(8, runningSum.ToByteArray().Length * 8 / 4));
                if (delta <= 0) delta = 1;

                // Ключевое свойство: seq[i] > сумма всех предыдущих
                seq[i] = runningSum + delta;
                runningSum += seq[i];
            }

            return seq;
        }

        // =====================================================
        // ШИФРОВАНИЕ
        // =====================================================

        /// <summary>
        /// Зашифрование сообщения.
        /// 
        /// Алгоритм:
        /// 1. Преобразуем текст в массив бит
        /// 2. Разбиваем биты на блоки по z бит
        /// 3. Каждый блок "укладываем в ранец":
        ///    S = Σ (b[i] * e[i]), где b[i] — бит, e[i] — элемент открытого ключа
        /// 4. Получаем массив чисел (весов ранцев) — это и есть шифртекст
        /// 
        /// Пример для блока 11010000 и ключа {62, 93, 186, 403, 417, 352, 315, 210}:
        /// S = 1*62 + 1*93 + 0*186 + 1*403 + 0*417 + 0*352 + 0*315 + 0*210 = 558
        /// </summary>
        /// <param name="plaintext">Открытый текст</param>
        /// <param name="useBase64">true — кодировка Base64, false — ASCII</param>
        /// <returns>Массив чисел (шифртекст)</returns>
        public BigInteger[] Encrypt(string plaintext, bool useBase64)
        {
            // Преобразуем текст в массив бит
            bool[] bits = TextToBits(plaintext, useBase64);

            // Дополняем до кратности z нулями (padding)
            int paddedLength = bits.Length;
            if (paddedLength % Z != 0)
                paddedLength += Z - (paddedLength % Z);

            bool[] paddedBits = new bool[paddedLength];
            Array.Copy(bits, paddedBits, bits.Length);
            // Оставшиеся биты уже false (0) — это padding

            // Количество блоков
            int blockCount = paddedLength / Z;
            BigInteger[] cipherBlocks = new BigInteger[blockCount];

            // Шифруем каждый блок — "укладываем ранец"
            for (int block = 0; block < blockCount; block++)
            {
                BigInteger sum = BigInteger.Zero;
                for (int i = 0; i < Z; i++)
                {
                    if (paddedBits[block * Z + i])
                    {
                        // Бит = 1 → "кладём предмет в ранец"
                        // Прибавляем соответствующий элемент ОТКРЫТОГО ключа
                        sum += PublicKey[i];
                    }
                }
                cipherBlocks[block] = sum;
            }

            return cipherBlocks;
        }

        // =====================================================
        // РАСШИФРОВАНИЕ
        // =====================================================

        /// <summary>
        /// Расшифрование сообщения.
        /// 
        /// Алгоритм:
        /// 1. Для каждого блока шифртекста вычисляем: S' = c * a⁻¹ mod n
        ///    Это преобразует «трудный ранец» обратно в «лёгкий»
        /// 2. Решаем задачу о ранце с сверхвозрастающей последовательностью (тайный ключ):
        ///    идём с конца, жадно забираем элементы
        /// 3. Восстанавливаем биты → символы → текст
        /// </summary>
        /// <param name="cipherBlocks">Массив чисел (шифртекст)</param>
        /// <param name="useBase64">true — Base64, false — ASCII</param>
        /// <returns>Расшифрованный текст</returns>
        public string Decrypt(BigInteger[] cipherBlocks, bool useBase64)
        {
            List<bool> allBits = new List<bool>();

            foreach (var block in cipherBlocks)
            {
                // Шаг 1: Обратное преобразование
                // S' = c * a⁻¹ mod n
                // Это «возвращает» нас к сверхвозрастающему ранцу
                BigInteger S = (block * AInverse) % N;

                // Шаг 2: Решаем задачу о ранце (сверхвозрастающий — лёгкий случай)
                bool[] blockBits = SolveSuperIncreasingKnapsack(S);
                allBits.AddRange(blockBits);
            }

            // Шаг 3: Биты → текст
            return BitsToText(allBits.ToArray(), useBase64);
        }

        /// <summary>
        /// Решение задачи о ранце для сверхвозрастающей последовательности.
        /// 
        /// Алгоритм (жадный, идём с конца):
        /// 1. Берём самый тяжёлый предмет (последний элемент d)
        /// 2. Если S >= d[i] → кладём предмет (бит = 1), S = S - d[i]
        /// 3. Если S < d[i] → не кладём (бит = 0)
        /// 4. Переходим к предыдущему элементу
        /// 5. Если S = 0 — решение найдено
        /// 
        /// Это работает ТОЛЬКО для сверхвозрастающей последовательности!
        /// Для произвольной последовательности жадный подход не гарантирует решение.
        /// </summary>
        private bool[] SolveSuperIncreasingKnapsack(BigInteger S)
        {
            bool[] bits = new bool[Z];

            // Идём от старшего (последнего) элемента к младшему
            for (int i = Z - 1; i >= 0; i--)
            {
                if (S >= PrivateKey[i])
                {
                    bits[i] = true;   // Кладём предмет в ранец
                    S -= PrivateKey[i]; // Уменьшаем оставшийся вес
                }
                else
                {
                    bits[i] = false;  // Не кладём
                }
            }

            // Если S != 0, значит что-то пошло не так
            // (ошибка в ключах или данных)

            return bits;
        }

        // =====================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ: ПРЕОБРАЗОВАНИЕ ТЕКСТА В БИТЫ
        // =====================================================

        /// <summary>
        /// Преобразование текста в массив бит.
        /// 
        /// Для ASCII: каждый символ → 8 бит (стандартная таблица)
        /// Для Base64: текст → Base64-строка → каждый символ Base64 → 6 бит
        /// </summary>
        private bool[] TextToBits(string text, bool useBase64)
        {
            if (useBase64)
            {
                // Шаг 1: Текст → байты → Base64-строка
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                string base64 = Convert.ToBase64String(textBytes);

                // Шаг 2: Каждый символ Base64 → 6 бит
                // Base64 использует 64 символа, каждый кодируется 6 битами
                List<bool> bits = new List<bool>();
                foreach (char c in base64)
                {
                    int value = Base64CharToIndex(c);
                    // Преобразуем индекс в 6 бит (старший бит первый)
                    for (int b = 5; b >= 0; b--)
                    {
                        bits.Add(((value >> b) & 1) == 1);
                    }
                }
                return bits.ToArray();
            }
            else
            {
                // ASCII: каждый символ → 8 бит
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                List<bool> bits = new List<bool>();
                foreach (byte bt in bytes)
                {
                    for (int b = 7; b >= 0; b--)
                    {
                        bits.Add(((bt >> b) & 1) == 1);
                    }
                }
                return bits.ToArray();
            }
        }

        /// <summary>
        /// Обратное преобразование: массив бит → текст.
        /// </summary>
        private string BitsToText(bool[] bits, bool useBase64)
        {
            if (useBase64)
            {
                // Каждые 6 бит → символ Base64
                int charCount = bits.Length / 6;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < charCount; i++)
                {
                    int value = 0;
                    for (int b = 0; b < 6; b++)
                    {
                        if (bits[i * 6 + b])
                            value |= (1 << (5 - b));
                    }
                    char c = Base64IndexToChar(value);
                    sb.Append(c);
                }

                // Base64-строка → байты → текст
                string base64Str = sb.ToString();
                // Добавляем padding '=' если нужно
                while (base64Str.Length % 4 != 0)
                    base64Str += "=";

                try
                {
                    byte[] decoded = Convert.FromBase64String(base64Str);
                    return Encoding.UTF8.GetString(decoded);
                }
                catch
                {
                    return "[Ошибка декодирования Base64]";
                }
            }
            else
            {
                // Каждые 8 бит → байт → символ
                int byteCount = bits.Length / 8;
                byte[] bytes = new byte[byteCount];
                for (int i = 0; i < byteCount; i++)
                {
                    byte val = 0;
                    for (int b = 0; b < 8; b++)
                    {
                        if (bits[i * 8 + b])
                            val |= (byte)(1 << (7 - b));
                    }
                    bytes[i] = val;
                }

                // Убираем нулевые байты в конце (padding)
                int actualLength = bytes.Length;
                while (actualLength > 0 && bytes[actualLength - 1] == 0)
                    actualLength--;

                return Encoding.UTF8.GetString(bytes, 0, actualLength);
            }
        }

        // =====================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ: BASE64
        // =====================================================

        /// <summary>
        /// Таблица символов Base64.
        /// Индексы 0-63 соответствуют символам A-Z, a-z, 0-9, +, /
        /// </summary>
        private const string Base64Chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private int Base64CharToIndex(char c)
        {
            if (c == '=') return 0; // Padding символ
            int idx = Base64Chars.IndexOf(c);
            return idx >= 0 ? idx : 0;
        }

        private char Base64IndexToChar(int index)
        {
            if (index >= 0 && index < 64)
                return Base64Chars[index];
            return 'A';
        }

        // =====================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ: МАТЕМАТИКА С БОЛЬШИМИ ЧИСЛАМИ
        // =====================================================

        /// <summary>
        /// Генерация случайного большого числа заданной битовой длины.
        /// Используем криптографически стойкий генератор.
        /// </summary>
        private BigInteger GenerateRandomBigInteger(int bits)
        {
            if (bits <= 0) bits = 8;
            int bytes = (bits + 7) / 8;
            byte[] data = new byte[bytes + 1]; // +1 для знакового байта
            rng.GetBytes(data, 0, bytes);

            // Обнуляем лишние биты в старшем байте
            int excessBits = bytes * 8 - bits;
            if (excessBits > 0 && bytes > 0)
            {
                data[bytes - 1] &= (byte)(0xFF >> excessBits);
            }

            // Устанавливаем старший бит, чтобы число было нужной длины
            if (bytes > 0)
            {
                int topBitPos = (bits - 1) % 8;
                data[bytes - 1] |= (byte)(1 << topBitPos);
            }

            data[bytes] = 0; // Знаковый байт = 0 → число положительное
            return new BigInteger(data);
        }

        /// <summary>
        /// Поиск числа a, взаимно простого с n (НОД(a, n) = 1).
        /// Генерируем случайные числа, пока не найдём подходящее.
        /// </summary>
        private BigInteger GenerateCoprime(BigInteger n)
        {
            while (true)
            {
                BigInteger a = GenerateRandomBigInteger(
                    Math.Max(32, n.ToByteArray().Length * 8 - 8));

                // a должно быть в диапазоне [2, n-1]
                a = (a % (n - 2)) + 2;

                // Проверяем взаимную простоту
                if (BigInteger.GreatestCommonDivisor(a, n) == 1)
                    return a;
            }
        }

        /// <summary>
        /// Вычисление обратного элемента: a⁻¹ mod n.
        /// Используем расширенный алгоритм Евклида.
        /// 
        /// Расширенный алгоритм Евклида находит x, y такие, что:
        /// a*x + n*y = НОД(a, n) = 1
        /// Тогда x mod n = a⁻¹ mod n
        /// 
        /// Это ключевой элемент расшифрования!
        /// </summary>
        public static BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger t = 0, newT = 1;
            BigInteger r = n, newR = a;

            while (newR != 0)
            {
                BigInteger quotient = r / newR;

                BigInteger tempT = t - quotient * newT;
                t = newT;
                newT = tempT;

                BigInteger tempR = r - quotient * newR;
                r = newR;
                newR = tempR;
            }

            if (r > 1)
                throw new ArithmeticException("Обратный элемент не существует");

            if (t < 0)
                t += n;

            return t;
        }

        // =====================================================
        // ИНФОРМАЦИОННЫЕ МЕТОДЫ (для отображения в интерфейсе)
        // =====================================================

        /// <summary>
        /// Проверка, является ли последовательность сверхвозрастающей.
        /// Используется для отладки и демонстрации.
        /// </summary>
        public bool IsSuperIncreasing(BigInteger[] seq)
        {
            BigInteger sum = BigInteger.Zero;
            for (int i = 0; i < seq.Length; i++)
            {
                if (i > 0 && seq[i] <= sum)
                    return false;
                sum += seq[i];
            }
            return true;
        }

        /// <summary>
        /// Возвращает строковое представление ключа (для отображения).
        /// </summary>
        public string KeyToString(BigInteger[] key)
        {
            return string.Join(", ", key.Select(k => k.ToString()));
        }
    }
}