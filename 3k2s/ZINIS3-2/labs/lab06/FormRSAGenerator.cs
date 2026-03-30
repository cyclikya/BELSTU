using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace lab06
{
    public partial class FormRSAGenerator : Form
    {
        public FormRSAGenerator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Генерация 256-разрядных параметров p, q, e для RSA-генератора ПСП.
        /// 
        /// Обоснование выбора:
        /// - p и q: случайные 256-битные простые числа.
        ///   256 бит обеспечивают n = p*q порядка 512 бит,
        ///   что делает факторизацию n вычислительно сложной.
        /// - e: берём стандартное значение 65537 (простое число Ферма F4).
        ///   Оно взаимно просто с (p-1)(q-1) в подавляющем большинстве случаев,
        ///   обеспечивает быстрое возведение в степень (всего 17 бит, два единичных).
        /// - seed (x0): случайное число в диапазоне [2, n-1].
        /// </summary>
        private void btnGenParams_Click(object sender, EventArgs e)
        {
            try
            {
                rtbOutput.Text = "Генерация 256-битных простых чисел p и q...\r\n";
                rtbOutput.Refresh();

                Stopwatch sw = Stopwatch.StartNew();

                BigInteger p = GeneratePrime(256);
                BigInteger q = GeneratePrime(256);

                // Убеждаемся что p != q
                while (q == p)
                    q = GeneratePrime(256);

                BigInteger n = p * q;
                BigInteger phi = (p - 1) * (q - 1);

                // e = 65537 — стандартный выбор
                BigInteger eVal = 65537;
                while (BigInteger.GreatestCommonDivisor(eVal, phi) != 1)
                    eVal += 2;

                // seed — случайное число из [2, n-1]
                BigInteger seed = RandomBigIntegerInRange(2, n - 1);

                sw.Stop();

                txtP.Text = p.ToString();
                txtQ.Text = q.ToString();
                txtE.Text = eVal.ToString();
                txtSeed.Text = seed.ToString();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ПАРАМЕТРЫ СГЕНЕРИРОВАНЫ ===");
                sb.AppendLine("Время генерации: " + sw.Elapsed.TotalMilliseconds.ToString("F1") + " мс");
                sb.AppendLine();
                sb.AppendLine("p (" + GetBitLength(p) + " бит):");
                sb.AppendLine(p.ToString());
                sb.AppendLine();
                sb.AppendLine("q (" + GetBitLength(q) + " бит):");
                sb.AppendLine(q.ToString());
                sb.AppendLine();
                sb.AppendLine("n = p*q (" + GetBitLength(n) + " бит):");
                sb.AppendLine(n.ToString());
                sb.AppendLine();
                sb.AppendLine("e = " + eVal);
                sb.AppendLine("НОД(e, phi) = " + BigInteger.GreatestCommonDivisor(eVal, phi));

                rtbOutput.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Генерация ПСП на основе RSA.
        /// 
        /// Алгоритм:
        /// x[t] = x[t-1]^e mod n
        /// Выход на каждом шаге: младший бит x[t].
        /// 
        /// Безопасность опирается на сложность взлома RSA,
        /// т.е. на задачу факторизации числа n.
        /// </summary>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                BigInteger p = BigInteger.Parse(txtP.Text);
                BigInteger q = BigInteger.Parse(txtQ.Text);
                BigInteger eVal = BigInteger.Parse(txtE.Text);
                BigInteger seed = BigInteger.Parse(txtSeed.Text);
                int count = (int)nudCount.Value;

                BigInteger n = p * q;
                BigInteger phi = (p - 1) * (q - 1);

                if (BigInteger.GreatestCommonDivisor(eVal, phi) != 1)
                {
                    MessageBox.Show("e не взаимно просто с (p-1)(q-1)! Сгенерируйте параметры заново.");
                    return;
                }

                BigInteger x = seed % n;
                if (x < 2) x = 2;

                Stopwatch sw = Stopwatch.StartNew();

                int[] bits = new int[count];
                for (int i = 0; i < count; i++)
                {
                    // x[t] = x[t-1]^e mod n
                    x = BigInteger.ModPow(x, eVal, n);
                    // Выход: младший бит
                    bits[i] = (int)(x % 2);
                }

                sw.Stop();

                // Подсчёт статистики
                int ones = 0;
                foreach (int b in bits)
                    if (b == 1) ones++;
                int zeros = count - ones;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ГЕНЕРАЦИЯ ПСП НА ОСНОВЕ RSA ===");
                sb.AppendLine("Формула: x[t] = x[t-1]^e mod n");
                sb.AppendLine("Выход: младший бит x[t]");
                sb.AppendLine("Количество бит: " + count);
                sb.AppendLine("Время: " + sw.Elapsed.TotalMilliseconds.ToString("F3") + " мс");
                sb.AppendLine();

                sb.AppendLine("--- Биты ---");
                for (int i = 0; i < bits.Length; i++)
                {
                    sb.Append(bits[i]);
                    if ((i + 1) % 64 == 0) sb.AppendLine();
                    else if ((i + 1) % 8 == 0) sb.Append(" ");
                }
                sb.AppendLine();
                sb.AppendLine();

                // Байты (первые 50)
                int byteCount = Math.Min(count / 8, 50);
                if (byteCount > 0)
                {
                    sb.AppendLine("--- Байты (первые " + byteCount + ") ---");
                    for (int i = 0; i < byteCount; i++)
                    {
                        byte val = 0;
                        for (int b = 0; b < 8; b++)
                        {
                            if (bits[i * 8 + b] == 1)
                                val |= (byte)(1 << (7 - b));
                        }
                        sb.AppendFormat("{0,4}", val);
                        if ((i + 1) % 16 == 0) sb.AppendLine();
                    }
                    sb.AppendLine();
                }

                sb.AppendLine();
                sb.AppendLine("--- Статистика ---");
                sb.AppendLine("Нулей: " + zeros + " (" + (100.0 * zeros / count).ToString("F1") + "%)");
                sb.AppendLine("Единиц: " + ones + " (" + (100.0 * ones / count).ToString("F1") + "%)");
                sb.AppendLine("Идеальное соотношение: 50% / 50%");

                rtbOutput.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // ==================================================
        // Вспомогательные методы
        // ==================================================

        private static int GetBitLength(BigInteger value)
        {
            byte[] bytes = value.ToByteArray();
            int bitLen = (bytes.Length - 1) * 8;
            byte msb = bytes[bytes.Length - 1];
            while (msb > 0) { bitLen++; msb >>= 1; }
            return bitLen;
        }

        /// <summary>
        /// Генерация случайного простого числа заданной битовой длины.
        /// Используется тест Миллера-Рабина (20 раундов).
        /// </summary>
        private static BigInteger GeneratePrime(int bits)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            while (true)
            {
                byte[] bytes = new byte[bits / 8 + 1];
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] = 0; // положительное

                // Устанавливаем старший бит нужного байта
                int topByte = bits / 8 - 1;
                if (topByte >= 0)
                    bytes[topByte] |= (byte)(1 << ((bits - 1) % 8));
                bytes[0] |= 1; // нечётное

                BigInteger candidate = new BigInteger(bytes);
                if (candidate < 0) candidate = -candidate;
                if (candidate < 4) continue;

                if (IsProbablyPrime(candidate, 20))
                    return candidate;
            }
        }

        private static bool IsProbablyPrime(BigInteger n, int k)
        {
            if (n < 2) return false;
            if (n == 2 || n == 3) return true;
            if (n % 2 == 0) return false;

            BigInteger d = n - 1;
            int r = 0;
            while (d % 2 == 0) { d /= 2; r++; }

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            for (int i = 0; i < k; i++)
            {
                BigInteger a = RandomBigIntegerInRange(2, n - 2);
                BigInteger x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1) continue;
                bool found = false;
                for (int j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == n - 1) { found = true; break; }
                }
                if (!found) return false;
            }
            return true;
        }

        private static BigInteger RandomBigIntegerInRange(BigInteger min, BigInteger max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            BigInteger range = max - min;
            byte[] bytes = range.ToByteArray();
            byte[] rndBytes = new byte[bytes.Length];
            BigInteger result;
            do
            {
                rng.GetBytes(rndBytes);
                rndBytes[rndBytes.Length - 1] &= 0x7F;
                result = new BigInteger(rndBytes);
            } while (result < 0 || result > range);
            return result + min;
        }
    }
}