using lab07;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace lab07
{
    public partial class Form1 : Form
    {
        // Экземпляр движка шифрования
        private KnapsackCipherEngine engine = new KnapsackCipherEngine();

        // Сохраняем шифртекст между операциями
        private BigInteger[] currentCipherText = null;

        // Текущий режим кодировки
        private bool useBase64 = false;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// При смене кодировки автоматически подставляем рекомендуемое z.
        /// Base64 → z=6, ASCII → z=8
        /// </summary>
        private void cmbEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            useBase64 = (cmbEncoding.SelectedIndex == 1);
            nudZ.Value = useBase64 ? 6 : 8;
        }

        /// <summary>
        /// КНОПКА «ГЕНЕРАЦИЯ КЛЮЧЕЙ»
        /// 
        /// Создаёт пару ключей:
        /// 1. Тайный ключ (сверхвозрастающая последовательность)
        /// 2. Открытый ключ (нормальная последовательность)
        /// А также параметры n, a, a⁻¹
        /// </summary>
        private void btnGenerateKeys_Click(object sender, EventArgs e)
        {
            try
            {
                int z = (int)nudZ.Value;

                // Запускаем генерацию
                Stopwatch sw = Stopwatch.StartNew();
                engine.GenerateKeys(z, targetBits: 100);
                sw.Stop();

                // Выводим результаты
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("        ГЕНЕРАЦИЯ КЛЮЧЕЙ");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();

                sb.AppendLine($"Параметры: z = {z}, кодировка = {(useBase64 ? "Base64" : "ASCII")}");
                sb.AppendLine($"Время генерации: {sw.Elapsed.TotalMilliseconds:F3} мс");
                sb.AppendLine();

                sb.AppendLine("--- ТАЙНЫЙ КЛЮЧ (сверхвозрастающая последовательность) ---");
                sb.AppendLine($"d = [{engine.KeyToString(engine.PrivateKey)}]");
                sb.AppendLine($"Является сверхвозрастающей: {engine.IsSuperIncreasing(engine.PrivateKey)}");
                sb.AppendLine();

                sb.AppendLine("--- ПАРАМЕТРЫ ---");
                sb.AppendLine($"n = {engine.N}");
                sb.AppendLine($"a = {engine.A}");
                sb.AppendLine($"a⁻¹ = {engine.AInverse}");

                // Проверка: a * a⁻¹ mod n = 1
                BigInteger check = (engine.A * engine.AInverse) % engine.N;
                sb.AppendLine($"Проверка (a · a⁻¹ mod n = 1): {check}");
                sb.AppendLine();

                sb.AppendLine("--- ОТКРЫТЫЙ КЛЮЧ (нормальная последовательность) ---");
                sb.AppendLine($"e = [{engine.KeyToString(engine.PublicKey)}]");
                sb.AppendLine($"Является сверхвозрастающей: {engine.IsSuperIncreasing(engine.PublicKey)}");
                sb.AppendLine();

                rtbOutput.Text = sb.ToString();
                lblStatus.Text = "✓ Ключи успешно сгенерированы";
                lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                currentCipherText = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации ключей: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// КНОПКА «ЗАШИФРОВАТЬ»
        /// 
        /// Берёт текст из поля ввода, преобразует в биты,
        /// каждый блок бит "укладывает в ранец" с открытым ключом.
        /// </summary>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (engine.PublicKey == null)
            {
                MessageBox.Show("Сначала сгенерируйте ключи!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string plaintext = txtPlaintext.Text;
            if (string.IsNullOrEmpty(plaintext))
            {
                MessageBox.Show("Введите текст для шифрования!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Замер времени
                Stopwatch sw = Stopwatch.StartNew();
                currentCipherText = engine.Encrypt(plaintext, useBase64);
                sw.Stop();

                // Отображаем результат
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("        ЗАШИФРОВАНИЕ");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Открытый текст: \"{plaintext}\"");
                sb.AppendLine($"Кодировка: {(useBase64 ? "Base64" : "ASCII")}");
                sb.AppendLine($"Размер блока z: {engine.Z}");
                sb.AppendLine();

                sb.AppendLine("--- ШИФРТЕКСТ (массив чисел — весов ранцев) ---");
                sb.AppendLine($"Количество блоков: {currentCipherText.Length}");
                sb.AppendLine();
                for (int i = 0; i < currentCipherText.Length; i++)
                {
                    sb.AppendLine($"  Блок {i + 1}: {currentCipherText[i]}");
                }
                sb.AppendLine();

                // Компактная запись
                sb.AppendLine("Шифртекст (компактно):");
                sb.AppendLine(string.Join(" ", currentCipherText));
                sb.AppendLine();

                rtbOutput.Text += sb.ToString();

                lblTimeEncrypt.Text = $"Время шифрования: {sw.Elapsed.TotalMilliseconds:F3} мс";
                lblStatus.Text = "✓ Сообщение зашифровано";
                lblStatus.ForeColor = System.Drawing.Color.DarkBlue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка шифрования: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// КНОПКА «РАСШИФРОВАТЬ»
        /// 
        /// Берёт шифртекст, для каждого блока:
        /// 1. Умножает на a⁻¹ mod n (обратное преобразование)
        /// 2. Решает задачу о ранце с тайным ключом (сверхвозрастающая последовательность)
        /// 3. Восстанавливает биты → символы → текст
        /// </summary>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (currentCipherText == null)
            {
                MessageBox.Show("Сначала зашифруйте сообщение!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                string decrypted = engine.Decrypt(currentCipherText, useBase64);
                sw.Stop();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("        РАСШИФРОВАНИЕ");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Расшифрованный текст: \"{decrypted}\"");
                sb.AppendLine();

                // Сравниваем с оригиналом
                string original = txtPlaintext.Text;
                bool match = (decrypted == original);
                sb.AppendLine($"Совпадение с оригиналом: {(match ? "ДА ✓" : "НЕТ ✗")}");
                sb.AppendLine();

                rtbOutput.Text += sb.ToString();

                lblTimeDecrypt.Text = $"Время расшифрования: {sw.Elapsed.TotalMilliseconds:F3} мс";
                lblStatus.Text = match ? "✓ Расшифровано успешно" : "✗ Ошибка расшифрования";
                lblStatus.ForeColor = match ? System.Drawing.Color.DarkGreen : System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расшифрования: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// КНОПКА «АНАЛИЗ»
        /// 
        /// Проводит серию экспериментов с разными значениями z
        /// для обеих кодировок и измеряет время шифрования/расшифрования.
        /// 
        /// Это нужно для пункта 2 задания:
        /// "Проанализировать время выполнения операций при увеличении
        /// числа членов ключевой последовательности"
        /// </summary>
        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            string text = txtPlaintext.Text;
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Введите текст для анализа!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine("   АНАЛИЗ ВРЕМЕНИ ШИФРОВАНИЯ/РАСШИФРОВАНИЯ");
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine($"Текст: \"{text}\"");
            sb.AppendLine();

            // Тестируемые значения z
            int[] zValues = { 6, 8, 16, 32, 64, 128 };

            // Таблица результатов
            sb.AppendLine(String.Format("{0,-8} {1,-10} {2,-18} {3,-18} {4,-18} {5,-10}",
                "z", "Кодировка", "Генерация (мс)", "Шифрование (мс)",
                "Расшифрование (мс)", "Верно?"));
            sb.AppendLine(new string('-', 90));

            foreach (bool isBase64 in new[] { false, true })
            {
                string encName = isBase64 ? "Base64" : "ASCII";

                foreach (int z in zValues)
                {
                    try
                    {
                        var testEngine = new KnapsackCipherEngine();

                        // Генерация ключей
                        Stopwatch swGen = Stopwatch.StartNew();
                        testEngine.GenerateKeys(z, targetBits: 100);
                        swGen.Stop();

                        // Шифрование
                        Stopwatch swEnc = Stopwatch.StartNew();
                        var cipher = testEngine.Encrypt(text, isBase64);
                        swEnc.Stop();

                        // Расшифрование
                        Stopwatch swDec = Stopwatch.StartNew();
                        string decrypted = testEngine.Decrypt(cipher, isBase64);
                        swDec.Stop();

                        bool ok = (decrypted == text);

                        sb.AppendLine(String.Format("{0,-8} {1,-10} {2,-18:F4} {3,-18:F4} {4,-18:F4} {5,-10}",
                            z, encName,
                            swGen.Elapsed.TotalMilliseconds,
                            swEnc.Elapsed.TotalMilliseconds,
                            swDec.Elapsed.TotalMilliseconds,
                            ok ? "Да" : "Нет"));
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  z={z}, {encName}: ОШИБКА — {ex.Message}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("ВЫВОДЫ:");
            sb.AppendLine("• При увеличении z время генерации ключей растёт");
            sb.AppendLine("  (нужно генерировать больше элементов последовательности)");
            sb.AppendLine("• Время шифрования зависит от числа блоков");
            sb.AppendLine("  (при большем z — меньше блоков, но больше операций на блок)");
            sb.AppendLine("• Base64 даёт больше блоков при z=6 (6 бит/символ vs 8)");

            rtbOutput.Text = sb.ToString();
            lblStatus.Text = "✓ Анализ завершён";
            lblStatus.ForeColor = System.Drawing.Color.Purple;
        }
    }
}