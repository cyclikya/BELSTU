using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1 - Зашифровать");
            Console.WriteLine("2 - Расшифровать");
            Console.WriteLine("0 - Выход");

            string choice = Console.ReadLine();

            if (choice == "1")
                EncryptMenu();

            else if (choice == "2")
                DecryptMenu();

            else if (choice == "0")
                break;
        }
    }

    // ЗАШИФРОВАНИЕ
    static void EncryptMenu()
    {
        Console.WriteLine("Введите текст:");
        string text = Console.ReadLine();

        Console.WriteLine("Введите ключ 1:");
        string k1 = Console.ReadLine();

        Console.WriteLine("Введите ключ 2:");
        string k2 = Console.ReadLine();

        Console.WriteLine("Введите ключ 3:");
        string k3 = Console.ReadLine();

        byte[] data = Encoding.UTF8.GetBytes(text);

        Console.WriteLine("\nИсходный размер: " + data.Length + " байт");

        data = Pad(data);

        ShowBlocks(data);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        byte[] encrypted = EncryptEEE3(data, k1, k2, k3);

        sw.Stop();

        Console.WriteLine("\nРезультат шифрования:");
        Console.WriteLine(Convert.ToBase64String(encrypted));

        Console.WriteLine("Время шифрования: " + sw.ElapsedMilliseconds + " ms");
    }

    // =========================
    // РАСШИФРОВАНИЕ
    // =========================

    static void DecryptMenu()
    {
        Console.WriteLine("Введите шифртекст (Base64):");
        string cipher = Console.ReadLine();

        Console.WriteLine("Введите ключ 1:");
        string k1 = Console.ReadLine();

        Console.WriteLine("Введите ключ 2:");
        string k2 = Console.ReadLine();

        Console.WriteLine("Введите ключ 3:");
        string k3 = Console.ReadLine();

        byte[] data = Convert.FromBase64String(cipher);

        ShowBlocks(data);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        byte[] decrypted = DecryptEEE3(data, k1, k2, k3);

        sw.Stop();

        Console.WriteLine("\nРасшифрованный текст:");
        Console.WriteLine(Encoding.UTF8.GetString(decrypted).Trim('\0'));

        Console.WriteLine("Время расшифрования: " + sw.ElapsedMilliseconds + " ms");
    }

    // =========================
    // ДОПОЛНЕНИЕ БЛОКА
    // =========================

    static byte[] Pad(byte[] data)
    {
        int block = 8;

        int padding = block - (data.Length % block);

        if (padding == block)
            return data;

        byte[] result = new byte[data.Length + padding];

        Array.Copy(data, result, data.Length);

        Console.WriteLine("Добавлено байт дополнения: " + padding);

        return result;
    }

    // =========================
    // ВЫВОД БЛОКОВ
    // =========================

    static void ShowBlocks(byte[] data)
    {
        Console.WriteLine("\nРазделение на блоки (8 байт):");

        for (int i = 0; i < data.Length; i += 8)
        {
            byte[] block = new byte[8];
            Array.Copy(data, i, block, 0, 8);

            Console.WriteLine("Блок " + (i / 8 + 1) + ": " +
                BitConverter.ToString(block));
        }
    }

    // =========================
    // DES-EEE3
    // =========================

    static byte[] EncryptEEE3(byte[] data, string k1, string k2, string k3)
    {
        byte[] step1 = DESEncrypt(data, k1);
        byte[] step2 = DESEncrypt(step1, k2);
        byte[] step3 = DESEncrypt(step2, k3);

        return step3;
    }

    static byte[] DecryptEEE3(byte[] data, string k1, string k2, string k3)
    {
        byte[] step1 = DESDecrypt(data, k3);
        byte[] step2 = DESDecrypt(step1, k2);
        byte[] step3 = DESDecrypt(step2, k1);

        return step3;
    }

    // =========================
    // DES
    // =========================

    static byte[] DESEncrypt(byte[] data, string key)
    {
        using (DES des = DES.Create())
        {
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.None;

            des.Key = NormalizeKey(key);

            Console.WriteLine("\nПреобразованный ключ:");
            Console.WriteLine(BitConverter.ToString(des.Key));

            ICryptoTransform enc = des.CreateEncryptor();

            return enc.TransformFinalBlock(data, 0, data.Length);
        }
    }

    static byte[] DESDecrypt(byte[] data, string key)
    {
        using (DES des = DES.Create())
        {
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.None;

            des.Key = NormalizeKey(key);

            ICryptoTransform dec = des.CreateDecryptor();

            return dec.TransformFinalBlock(data, 0, data.Length);
        }
    }

    // =========================
    // НОРМАЛИЗАЦИЯ КЛЮЧА
    // =========================

    static byte[] NormalizeKey(string key)
    {
        byte[] result = new byte[8];

        byte[] src = Encoding.UTF8.GetBytes(key);

        Console.WriteLine("\nИсходный ключ (байты):");
        Console.WriteLine(BitConverter.ToString(src));

        for (int i = 0; i < 8; i++)
        {
            if (i < src.Length)
                result[i] = src[i];
            else
                result[i] = 0;
        }

        return result;
    }
}