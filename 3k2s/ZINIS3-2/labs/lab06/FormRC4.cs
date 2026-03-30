using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lab06
{
    public partial class FormRC4 : Form
    {
        // Таблица замен S — ядро RC4
        private int[] S;
        // Сохранённое состояние S после инициализации (для сброса)
        private int[] initialS;
        // Счётчики генератора
        private int gi, gj;
        // Флаг готовности
        private bool ready = false;
        // Шифртекст для расшифрования
        private byte[] cipherData = null;

        public FormRC4()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Инициализация RC4 (KSA — Key Scheduling Algorithm).
        /// 
        /// 1. S[0..255] = 0, 1, 2, ..., 255
        /// 2. Ключ повторяется циклически: K[i] = key[i % keyLen]
        /// 3. j=0; для i от 0 до 255: j=(j+S[i]+K[i]) mod 256; swap(S[i],S[j])
        /// 
        /// После этого S содержит перестановку 0..255, зависящую от ключа.
        /// </summary>
        private void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
                int[] key = ParseKey();

                // Проверка диапазона ключа для n=8: значения 0..255
                foreach (int k in key)
                {
                    if (k < 0 || k > 255)
                    {
                        MessageBox.Show("Значение ключа " + k + " вне диапазона [0, 255]");
                        return;
                    }
                }

                // Шаг 1: линейное заполнение S
                S = new int[256];
                for (int i = 0; i < 256; i++)
                    S[i] = i;

                // Шаг 2: массив K — ключ с повторениями
                int[] K = new int[256];
                for (int i = 0; i < 256; i++)
                    K[i] = key[i % key.Length];

                // Шаг 3: перемешивание S (KSA)
                int j = 0;
                for (int i = 0; i < 256; i++)
                {
                    j = (j + S[i] + K[i]) % 256;
                    int tmp = S[i]; S[i] = S[j]; S[j] = tmp;
                }

                // Сохраняем состояние
                initialS = new int[256];
                Array.Copy(S, initialS, 256);
                gi = 0; gj = 0;
                ready = true;
                cipherData = null;

                // Вывод
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ИНИЦИАЛИЗАЦИЯ RC4 (KSA) ===");
                sb.AppendLine("n = 8, размер таблицы = 256");
                sb.AppendLine("Ключ: [" + string.Join(", ", key) + "]");
                sb.AppendLine();
                sb.AppendLine("Таблица S после инициализации (первые 64 элемента):");
                for (int i = 0; i < 64; i++)
                {
                    sb.AppendFormat("{0,4}", S[i]);
                    if ((i + 1) % 16 == 0) sb.AppendLine();
                }
                sb.AppendLine("...");
                sb.AppendLine();
                sb.AppendLine("Инициализация завершена.");

                rtbOutput.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Сброс генератора в состояние сразу после KSA.
        /// Нужен чтобы расшифрование выдало ту же гамму что и шифрование.
        /// </summary>
        private void ResetState()
        {
            Array.Copy(initialS, S, 256);
            gi = 0; gj = 0;
        }

        /// <summary>
        /// Генерация одного байта гаммы (PRGA).
        /// 
        /// i = (i+1) mod 256
        /// j = (j + S[i]) mod 256
        /// swap(S[i], S[j])
        /// K = S[(S[i]+S[j]) mod 256]
        /// </summary>
        private byte NextKeyByte()
        {
            gi = (gi + 1) % 256;
            gj = (gj + S[gi]) % 256;
            int tmp = S[gi]; S[gi] = S[gj]; S[gj] = tmp;
            int idx = (S[gi] + S[gj]) % 256;
            return (byte)S[idx];
        }

        /// <summary>
        /// Шифрование: C[i] = M[i] XOR K[i]
        /// </summary>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (!ready)
            {
                MessageBox.Show("Сначала выполните инициализацию!");
                return;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(txtPlaintext.Text);
                ResetState();

                Stopwatch sw = Stopwatch.StartNew();
                cipherData = new byte[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    byte k = NextKeyByte();
                    cipherData[i] = (byte)(data[i] ^ k);
                }
                sw.Stop();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ШИФРОВАНИЕ RC4 ===");
                sb.AppendLine("Текст: \"" + txtPlaintext.Text + "\"");
                sb.AppendLine("Длина: " + data.Length + " байт");
                sb.AppendLine("Время: " + sw.Elapsed.TotalMilliseconds.ToString("F4") + " мс");
                sb.AppendLine();

                // Таблица: позиция | данные | гамма | результат
                sb.AppendLine(string.Format("{0,-6} {1,-8} {2,-8} {3,-8}",
                    "Поз.", "Вход", "Гамма", "Выход"));
                sb.AppendLine(new string('-', 35));

                // Повторяем для вывода гаммы
                ResetState();
                for (int i = 0; i < data.Length && i < 60; i++)
                {
                    byte k = NextKeyByte();
                    sb.AppendLine(string.Format("{0,-6} {1,-8} {2,-8} {3,-8}",
                        i, data[i], k, (byte)(data[i] ^ k)));
                }
                if (data.Length > 60)
                    sb.AppendLine("... (ещё " + (data.Length - 60) + " байт)");

                // Восстанавливаем состояние шифрования (чтобы расшифрование было корректным)
                // cipherData уже посчитан, больше состояние не нужно

                sb.AppendLine();
                sb.AppendLine("--- Шифртекст (HEX) ---");
                sb.AppendLine(BitConverter.ToString(cipherData).Replace("-", " "));
                sb.AppendLine();
                sb.AppendLine("--- Шифртекст (DEC) ---");
                sb.AppendLine(string.Join(" ", cipherData.Select(b => b.ToString())));

                rtbOutput.Text = sb.ToString();
                lblTimeEnc.Text = "Шифрование: " + sw.Elapsed.TotalMilliseconds.ToString("F4") + " мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Расшифрование: M[i] = C[i] XOR K[i]
        /// Та же самая операция что и шифрование (свойство XOR).
        /// Главное — сбросить генератор в начальное состояние.
        /// </summary>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (cipherData == null)
            {
                MessageBox.Show("Сначала зашифруйте сообщение!");
                return;
            }

            try
            {
                ResetState();

                Stopwatch sw = Stopwatch.StartNew();
                byte[] decrypted = new byte[cipherData.Length];
                for (int i = 0; i < cipherData.Length; i++)
                {
                    byte k = NextKeyByte();
                    decrypted[i] = (byte)(cipherData[i] ^ k);
                }
                sw.Stop();

                string result = Encoding.UTF8.GetString(decrypted);
                bool match = (result == txtPlaintext.Text);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== РАСШИФРОВАНИЕ RC4 ===");
                sb.AppendLine("Время: " + sw.Elapsed.TotalMilliseconds.ToString("F4") + " мс");
                sb.AppendLine();
                sb.AppendLine("Результат: \"" + result + "\"");
                sb.AppendLine();
                sb.AppendLine("Совпадение с оригиналом: " + (match ? "ДА" : "НЕТ"));

                rtbOutput.Text = sb.ToString();
                lblTimeDec.Text = "Расшифрование: " + sw.Elapsed.TotalMilliseconds.ToString("F4") + " мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Тест скорости генерации ПСП и шифрования для разных объёмов данных.
        /// </summary>
        private void btnSpeed_Click(object sender, EventArgs e)
        {
            if (!ready)
            {
                MessageBox.Show("Сначала выполните инициализацию!");
                return;
            }

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ОЦЕНКА СКОРОСТИ RC4 ===");
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-12} {1,-18} {2,-18} {3,-10}",
                    "Размер", "Генерация(мс)", "Шифрование(мс)", "МБ/с"));
                sb.AppendLine(new string('-', 60));

                int[] sizes = { 100, 1000, 10000, 100000, 1000000 };
                Random rnd = new Random(42);

                foreach (int size in sizes)
                {
                    byte[] testData = new byte[size];
                    rnd.NextBytes(testData);

                    // Тест генерации гаммы
                    ResetState();
                    Stopwatch swGen = Stopwatch.StartNew();
                    for (int i = 0; i < size; i++)
                        NextKeyByte();
                    swGen.Stop();

                    // Тест шифрования
                    ResetState();
                    Stopwatch swEnc = Stopwatch.StartNew();
                    byte[] enc = new byte[size];
                    for (int i = 0; i < size; i++)
                    {
                        byte k = NextKeyByte();
                        enc[i] = (byte)(testData[i] ^ k);
                    }
                    swEnc.Stop();

                    double mbps = 0;
                    if (swEnc.Elapsed.TotalSeconds > 0)
                        mbps = (size / 1048576.0) / swEnc.Elapsed.TotalSeconds;

                    sb.AppendLine(string.Format("{0,-12} {1,-18:F4} {2,-18:F4} {3,-10:F2}",
                        size, swGen.Elapsed.TotalMilliseconds,
                        swEnc.Elapsed.TotalMilliseconds, mbps));
                }

                sb.AppendLine();
                sb.AppendLine("Скорость RC4 линейно зависит от объёма данных.");
                sb.AppendLine("Каждый байт обрабатывается за O(1) операций.");

                rtbOutput.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private int[] ParseKey()
        {
            string[] parts = txtKey.Text.Split(new char[] { ',', ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            return parts.Select(p => int.Parse(p.Trim())).ToArray();
        }
    }
}