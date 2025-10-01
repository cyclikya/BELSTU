using System;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

class LR1
{
    static void Main()
    {




        //pr1a();
        //pr1b();
        //pr1c();
        //pr1d();
        //pr1e();

        //pr2a();
        //pr2b();
        //pr2c();
        //pr2d();

        //pr3a();
        //pr3b();
        //pr3c();
        //pr3d();

        //pr4();

        //pr5
        int[] numbers = { 3, 5, 7, 2, 8, 6 };
        string text = "Hello";
        var result = Get(numbers, text);

        Console.WriteLine($"Максимальный элемент: {result.Max}");
        Console.WriteLine($"Минимальный элемент: {result.Min}");
        Console.WriteLine($"Сумма элементов: {result.Sum}");
        Console.WriteLine($"Первая буква строки: {result.FirstLetter}");

        static (int Max, int Min, int Sum, char FirstLetter) Get(int[] array, string str)
        {
            int max = array.Max();
            int min = array.Min();
            int sum = array.Sum();
            char firstLetter = str[0];

            return (max, min, sum, firstLetter);
        }

        void f(string[] str)
        {
            Console.WriteLine(str[1]);
        }

        string[] x = { "gdfgh", "wghj" };
        f(x);

        //pr6();
    }

    static void pr1a()
    {
        // Ввод и вывод значений для различных типов данных
        Console.Write("Введите логическое значение (true/false): ");
        bool booleanValue = bool.Parse(Console.ReadLine());

        Console.Write("Введите число типа byte (от 0 до 255): ");
        byte byteValue = byte.Parse(Console.ReadLine());

        Console.Write("Введите число типа sbyte (от -128 до 127): ");
        sbyte sbyteValue = sbyte.Parse(Console.ReadLine());

        Console.Write("Введите один символ: ");
        char charValue = char.Parse(Console.ReadLine());

        Console.Write("Введите число типа decimal: ");
        decimal decimalValue = decimal.Parse(Console.ReadLine());

        Console.Write("Введите число типа double: ");
        double doubleValue = double.Parse(Console.ReadLine());

        Console.Write("Введите число типа float: ");
        float floatValue = float.Parse(Console.ReadLine());

        Console.Write("Введите число типа int: ");
        int intValue = int.Parse(Console.ReadLine());

        Console.Write("Введите число типа uint: ");
        uint uintValue = uint.Parse(Console.ReadLine());

        Console.Write("Введите число типа long: ");
        long longValue = long.Parse(Console.ReadLine());

        Console.Write("Введите число типа ulong: ");
        ulong ulongValue = ulong.Parse(Console.ReadLine());

        Console.Write("Введите число типа short: ");
        short shortValue = short.Parse(Console.ReadLine());

        Console.Write("Введите число типа ushort: ");
        ushort ushortValue = ushort.Parse(Console.ReadLine());

        Console.Write("Введите строку: ");
        string stringValue = Console.ReadLine();

        Console.WriteLine("\nВведённые значения:");
        Console.WriteLine($"bool: {booleanValue}");
        Console.WriteLine($"byte: {byteValue}");
        Console.WriteLine($"sbyte: {sbyteValue}");
        Console.WriteLine($"char: {charValue}");
        Console.WriteLine($"decimal: {decimalValue}");
        Console.WriteLine($"double: {doubleValue}");
        Console.WriteLine($"float: {floatValue}");
        Console.WriteLine($"int: {intValue}");
        Console.WriteLine($"uint: {uintValue}");
        Console.WriteLine($"long: {longValue}");
        Console.WriteLine($"ulong: {ulongValue}");
        Console.WriteLine($"short: {shortValue}");
        Console.WriteLine($"ushort: {ushortValue}");
        Console.WriteLine($"string: {stringValue}");
    }
    static void pr1b()
    {
        // Явное приведение типов
        double doubleValue = 9.97;
        int intValue = (int)doubleValue;
        Console.WriteLine($"double to int: {intValue}");

        float floatValue = 12.34f;
        int intValueFromFloat = (int)floatValue;
        Console.WriteLine($"float to int: {intValueFromFloat}");

        long longValue = 100000L;
        int intValueFromLong = (int)longValue;
        Console.WriteLine($"long to int: {intValueFromLong}");

        int intValue2 = 300;
        byte byteValue = (byte)intValue2;
        Console.WriteLine($"int to byte: {byteValue}");

        char charValue = 'A';
        int intFromChar = (int)charValue;
        Console.WriteLine($"char to int: {intFromChar}");

        // Неявное приведение типов
        int NintValue = 123;
        long NlongValue = NintValue;
        Console.WriteLine($"int to long: {NlongValue}");

        float NfloatValue = 3.14f;
        double NdoubleValue = NfloatValue;
        Console.WriteLine($"float to double: {NdoubleValue}");

        byte NbyteValue = 255;
        int NintFromByte = NbyteValue;
        Console.WriteLine($"byte to int: {NintFromByte}");

        short shortValue = 32000;
        int intFromShort = shortValue;
        Console.WriteLine($"short to int: {intFromShort}");

        char NcharValue = 'B';
        int NintFromChar = NcharValue;
        Console.WriteLine($"char to int: {NintFromChar}");
    }
    static void pr1c()
    {
        //Выполните упаковку и распаковку значимых типов.
        int x = 9;
        object xobj = x;
        int y = (int)xobj;
    }
    static void pr1d()
    {
        //неявно типизированные переменные
        var x = 9;
        var y = 'N';
        Console.WriteLine($"x = {x}, type = {x.GetType()}");
        Console.WriteLine($"y = {y}, type = {y.GetType()}");
    }
    static void pr1e()
    {
        //nullable переменные
        int? x = null;
        Console.WriteLine($"x = {x}, type = {x.GetType()}");
        x = 9;
        Console.WriteLine($"x = {x}, type = {x.GetType()}");

    }
    static void pr1f()
    {
        var x = 5;
        Console.WriteLine($"x = {x}, type = {x.GetType()}");
        //x = "g";
    }

    static void pr2a()
    {
        string x = "Hello";
        string y = "hello";
        Console.WriteLine(x == y);
    }
    static void pr2b()
    {
        string x = "Hello World";
        string y = "Hey";
        string z = " C#";

        Console.WriteLine($"сцепление: {x.Concat(y)}");
        string c = string.Copy(x);
        Console.WriteLine($"копирование: {c}");
        Console.WriteLine($"выделение подстроки {z.Substring(0, 1)}");
        string[] m = x.Split(' '); //разделение строки на слова
        string t = z.Insert(0, y); //вставки подстроки в заданную позицию
        Console.WriteLine($"удаление заданной подстроки : {x.Remove(5, 10)}");


        string name = "Alice";
        int age = 30;

        string human = $"Name: {name}, Age: {age}";
    }
    static void pr2c()
    {
        string x = String.Empty;
        string? y = null;

        Console.WriteLine($"Empty: {string.IsNullOrEmpty(x)}");
        Console.WriteLine($"Null: {string.IsNullOrEmpty(y)}");
    }
    static void pr2d()
    {
        StringBuilder sb = new StringBuilder("Hello, World!");

        Console.WriteLine($"Исходная строка: {sb}");

        sb.Remove(5, 2);
        Console.WriteLine($"После удаления: {sb}");

        sb.Insert(0, "Greeting: "); // В начале
        sb.Append(" Have a nice day!"); // В конце

        Console.WriteLine($"После добавления символов: {sb}");
    }
    static void pr3a()
    {
        int[,] x = {
        {7, 8, 3, 4},
        {9, 4, 0, 7},
        {6, 8, 5, 9}};

        for (int i = 0; i < x.GetLength(0); i++)
        {
            for (int j = 0; j < x.GetLength(1); j++)
            {
                Console.Write($"{x[i, j]} ");
            }
            Console.WriteLine();
        };
    }
    static void pr3b()
    {
        string[] array = { "Apple", "Banana", "Cherry", "Date", "Elderberry" };

        Console.WriteLine("Содержимое массива:");
        for (int i = 0; i < array.Length; i++)
        {
            Console.WriteLine($"Элемент {i}: {array[i]}");
        }

        Console.WriteLine($"\nДлина массива: {array.Length}");

        Console.Write("\nВведите индекс элемента для изменения: ");
        int index = int.Parse(Console.ReadLine());

        if (index >= 0 && index < array.Length)
        {
            Console.Write("Введите новое значение: ");
            string newValue = Console.ReadLine();

            array[index] = newValue;

            Console.WriteLine("\nИзменённое содержимое массива:");
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine($"Элемент {i}: {array[i]}");
            }
        }
        else
        {
            Console.WriteLine("Ошибка: индекс вне допустимого диапазона.");
        }
    }
    static void pr3c()
    {
        int[][] jaggedArray = new int[3][];
        jaggedArray[0] = new int[2];
        jaggedArray[1] = new int[3];
        jaggedArray[2] = new int[4];

        Console.WriteLine("Введите значения для ступенчатого массива:");

        for (int i = 0; i < jaggedArray.Length; i++)
        {
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                Console.Write($"Введите значение для элемента [{i}, {j}]: ");
                jaggedArray[i][j] = int.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("\nВыведенные значения ступенчатого массива:");
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                Console.Write($"{jaggedArray[i][j]} ");
            }
            Console.WriteLine();
        }
    }
    static void pr3d()
    {
        var intArray = new int[] { 1, 2, 3, 4, 5 };

        var message = "Hello, World!";

        Console.WriteLine("Массив целых чисел:");
        foreach (var number in intArray)
        {
            Console.WriteLine(number);
        }

        Console.WriteLine("\nСообщение:");
        Console.WriteLine(message);
    }


   static void pr4()
    {
        var myTuple = (1, "Hello", 'A', "World", 1234567890123456789UL);

        // a.	Задайте кортеж из 5 элементов с типами int, string, char, string, ulong. 
        // b.Выведите кортеж на консоль целиком и выборочно(например 1, 3, 4  элементы)
        Console.WriteLine($"Int: {myTuple.Item1}");
        Console.WriteLine($"Char: {myTuple.Item3}");
        Console.WriteLine($"String2: {myTuple.Item4}");

        //c Выполните распаковку кортежа в переменные.Продемонстрируйте различные способы распаковки кортежа.Продемонстрируйте использование переменной(_). (доступно начиная с C#7.3)

        (int id, string greeting, char initial, string message, ulong largeNumber) = myTuple;

        (int idOnly, _, char initialOnly, _, _) = myTuple;//пропуская некоторые переменные

        var newArray = new[] { myTuple.Item2, myTuple.Item4 };//массив из кортежа

        //d	Создайте неявно типизированные переменные для хранения массива и строки.
        var tuple1 = (Max: 10, Min: 1, Sum: 25, FirstLetter: 'H');
        var tuple2 = (Max: 10, Min: 1, Sum: 25, FirstLetter: 'H');
        var tuple3 = (Max: 12, Min: 2, Sum: 30, FirstLetter: 'W');

        // Сравнение кортежей
        bool areEqual1 = tuple1.Equals(tuple2); 
        bool areEqual2 = tuple1.Equals(tuple3);
    }
   static void pr6()
    {
        CheckedFunction();
        UncheckedFunction();

        void CheckedFunction()
        {
            try
            {
                checked
                {
                    int maxValue = int.MaxValue;
                    Console.WriteLine("Checked блок: int.MaxValue = " + maxValue);
                    maxValue++;
                    Console.WriteLine("Checked блок: После увеличения на 1 = " + maxValue);
                }
            }
            catch (OverflowException ex)
            {
                Console.WriteLine("Checked блок: Переполнение! " + ex.Message);
            }
        }

        void UncheckedFunction()
        {
            unchecked
            {
                int maxValue = int.MaxValue;
                Console.WriteLine("Unchecked блок: int.MaxValue = " + maxValue);
                maxValue++;
                Console.WriteLine("Unchecked блок: После увеличения на 1 = " + maxValue);
            }
        }
    }

}





