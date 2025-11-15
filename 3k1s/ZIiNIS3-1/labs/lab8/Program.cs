using LB88;
using System;
using System.Diagnostics;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        RunBWTTest("Виолетта");
        RunBWTTest("Угоренко");
        RunBWTTest("достопримечательность");

        string input = "дос";
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding(866);
        byte[] bytes = encoding.GetBytes(input);
        string binaryString = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

        Console.WriteLine("\n3 первых символа слова по варианту: " + input);
        Console.WriteLine("Бинарное представление по ASCII: " + binaryString + "\n");

        RunBWTTest(binaryString);
    }

    public static void RunBWTTest(string input)
    {
        Stopwatch codingTime = new Stopwatch();
        Stopwatch decodingTime = new Stopwatch();

        Console.WriteLine($"-----------------------Работа со строкой: {input}");
        Console.WriteLine($"M = {input} | k = {input.Length}");

        // === Прямое преобразование (сжатие) ===
        codingTime.Start();

        Console.WriteLine($"1. Формируем таблицу W1 ({input.Length}x{input.Length}) — все циклические сдвиги:");
        string[] W1 = BWT.GetShiftsMatrixW1(input);
        MatrixOperations.PrintMatrix(W1);

        Console.WriteLine("2. Сортируем строки таблицы W1 и получаем W2:");
        string[] W2 = BWT.GetSortMatrixW(W1);
        MatrixOperations.PrintMatrix(W2);

        Console.WriteLine("3. Извлекаем последний столбец Mk и вычисляем позицию исходной строки z:");
        string Mk = BWT.GetLastColumnMk(W2);
        int z = BWT.GetZRowPosition(input, W2) + 1;
        string encoded = Mk + z;

        Console.WriteLine($"Mk = {Mk}\nz = {z}");
        Console.WriteLine($"Результат кодирования: {encoded}");

        codingTime.Stop();

        // === Обратное преобразование (декодирование) ===
        Console.WriteLine("\n\tОбратное преобразование\n");
        decodingTime.Start();

        int matrixWLength = input.Length;
        string gettedMessage = encoded.Substring(0, matrixWLength);

        Console.WriteLine($"4. Восстанавливаем таблицу по Mk = {gettedMessage}");
        string[] W = BWT.GetDecodingMatrix(gettedMessage);
        MatrixOperations.PrintMatrix(W);

        int zNum = int.Parse(encoded.Substring(input.Length));
        string decoded = BWT.GetZRowM(W, zNum);

        Console.WriteLine($"Имея z = {zNum}, восстанавливаем исходное сообщение: M = '{decoded}'");

        decodingTime.Stop();

        Console.WriteLine($"\nВремя прямого преобразования: {codingTime.ElapsedMilliseconds} мс");
        Console.WriteLine($"Время обратного преобразования: {decodingTime.ElapsedMilliseconds} мс\n");
    }
}
