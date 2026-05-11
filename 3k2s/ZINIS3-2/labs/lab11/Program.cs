using System;
using System.Collections.Generic;

class Program
{
    const int Pmod = 751;
    const int A = -1;

    struct Point
    {
        public int X, Y;
        public bool Inf;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
            Inf = false;
        }

        public override string ToString()
        {
            return Inf ? "O" : $"({X}, {Y})";
        }

        public static Point Infinity()
        {
            return new Point { Inf = true };
        }
    }

    static Dictionary<char, Point> letters = new Dictionary<char, Point>()
    {
        {'А', new Point(189, 297)}, {'Б', new Point(189, 454)},
        {'В', new Point(192, 32)},  {'Г', new Point(192, 719)},
        {'Д', new Point(194, 205)}, {'Е', new Point(194, 546)},
        {'Ж', new Point(197, 145)}, {'З', new Point(197, 606)},
        {'И', new Point(198, 224)}, {'Й', new Point(198, 527)},
        {'К', new Point(200, 30)},  {'Л', new Point(200, 721)},
        {'М', new Point(203, 324)}, {'Н', new Point(203, 427)},
        {'О', new Point(205, 372)}, {'П', new Point(205, 379)},
        {'Р', new Point(206, 106)}, {'С', new Point(206, 645)},
        {'Т', new Point(209, 82)},  {'У', new Point(209, 669)},
        {'Ф', new Point(210, 31)},  {'Х', new Point(210, 720)},
        {'Ц', new Point(215, 247)}, {'Ч', new Point(215, 504)},
        {'Ш', new Point(218, 150)}, {'Щ', new Point(218, 601)},
        {'Ъ', new Point(221, 138)}, {'Ы', new Point(221, 613)},
        {'Ь', new Point(226, 9)},   {'Э', new Point(226, 742)},
        {'Ю', new Point(227, 299)}, {'Я', new Point(227, 452)}
    };

    static int Mod(int x, int m = Pmod)
    {
        x %= m;
        if (x < 0) x += m;
        return x;
    }

    static int Inv(int a, int m)
    {
        a = Mod(a, m);

        for (int x = 1; x < m; x++)
            if (Mod(a * x, m) == 1)
                return x;

        throw new Exception("Обратного числа нет");
    }

    static Point Add(Point p, Point q)
    {
        if (p.Inf) return q;
        if (q.Inf) return p;

        if (p.X == q.X && Mod(p.Y + q.Y) == 0)
            return Point.Infinity();

        int lambda;

        if (p.X == q.X && p.Y == q.Y)
            lambda = Mod((3 * p.X * p.X + A) * Inv(2 * p.Y, Pmod));
        else
            lambda = Mod((q.Y - p.Y) * Inv(q.X - p.X, Pmod));

        int x3 = Mod(lambda * lambda - p.X - q.X);
        int y3 = Mod(lambda * (p.X - x3) - p.Y);

        return new Point(x3, y3);
    }

    static Point Neg(Point p)
    {
        if (p.Inf) return p;
        return new Point(p.X, Mod(-p.Y));
    }

    static Point Mul(int k, Point p)
    {
        Point result = Point.Infinity();

        for (int i = 0; i < k; i++)
            result = Add(result, p);

        return result;
    }

    static List<Point> FindPoints(int xmin, int xmax)
    {
        List<Point> points = new List<Point>();

        for (int x = xmin; x <= xmax; x++)
        {
            int right = Mod(x * x * x - x + 1);

            for (int y = 0; y < Pmod; y++)
                if (Mod(y * y) == right)
                    points.Add(new Point(x, y));
        }

        return points;
    }

    static char FindLetter(Point p)
    {
        foreach (var item in letters)
            if (item.Value.X == p.X && item.Value.Y == p.Y)
                return item.Key;

        return '?';
    }

    static void Task1()
    {
        int k = 6;
        int l = 7;

        List<Point> points = FindPoints(621, 655);

        Console.WriteLine("Точки кривой:");
        foreach (Point point in points)
            Console.WriteLine(point);

        Point P = points[0];
        Point Q = points[2];
        Point R = points[4];

        Console.WriteLine("\nP = " + P);
        Console.WriteLine("Q = " + Q);
        Console.WriteLine("R = " + R);

        Console.WriteLine("\nkP = " + Mul(k, P));
        Console.WriteLine("P + Q = " + Add(P, Q));
        Console.WriteLine("kP + lQ - R = " + Add(Add(Mul(k, P), Mul(l, Q)), Neg(R)));
        Console.WriteLine("P - Q + R = " + Add(Add(P, Neg(Q)), R));
    }

    static void Task2()
    {
        string text = "УГОРЕНКО";

        Point G = new Point(0, 1);
        int d = 51;
        int k = 7;

        Point Q = Mul(d, G);

        Console.WriteLine("Исходный текст: " + text);
        Console.WriteLine("G = " + G);
        Console.WriteLine("d = " + d);
        Console.WriteLine("Открытый ключ Q = dG = " + Q);
        Console.WriteLine();

        List<Point> c1List = new List<Point>();
        List<Point> c2List = new List<Point>();

        foreach (char ch in text)
        {
            Point M = letters[ch];

            Point C1 = Mul(k, G);
            Point C2 = Add(M, Mul(k, Q));

            c1List.Add(C1);
            c2List.Add(C2);

            Console.WriteLine($"{ch}: M = {M}, C1 = {C1}, C2 = {C2}");
        }

        Console.WriteLine("\nРасшифрование:");

        string result = "";

        for (int i = 0; i < c1List.Count; i++)
        {
            Point M = Add(c2List[i], Neg(Mul(d, c1List[i])));
            char ch = FindLetter(M);

            result += ch;
            Console.WriteLine($"M = {M}, буква = {ch}");
        }

        Console.WriteLine("\nРасшифрованный текст: " + result);
    }

    static void Task3()
    {
        Point G = new Point(416, 55);
        int q = 13;
        int d = 11;
        int k = 7;

        char firstLetter = 'Л';
        int h = letters[firstLetter].X % q;

        Point Q = Mul(d, G);

        Point kG = Mul(k, G);
        int r = kG.X % q;
        int s = Mod(Inv(k, q) * (h + d * r), q);

        Console.WriteLine("ECDSA");
        Console.WriteLine("G = " + G);
        Console.WriteLine("q = " + q);
        Console.WriteLine("d = " + d);
        Console.WriteLine("Q = dG = " + Q);
        Console.WriteLine("H(M) = " + h);
        Console.WriteLine("k = " + k);

        Console.WriteLine("\nПодпись:");
        Console.WriteLine("r = " + r);
        Console.WriteLine("s = " + s);

        int w = Inv(s, q);
        int u1 = Mod(h * w, q);
        int u2 = Mod(r * w, q);

        Point checkPoint = Add(Mul(u1, G), Mul(u2, Q));
        int v = checkPoint.X % q;

        Console.WriteLine("\nПроверка:");
        Console.WriteLine("w = " + w);
        Console.WriteLine("u1 = " + u1);
        Console.WriteLine("u2 = " + u2);
        Console.WriteLine("Точка проверки = " + checkPoint);
        Console.WriteLine("v = " + v);

        if (v == r)
            Console.WriteLine("Подпись верна");
        else
            Console.WriteLine("Подпись неверна");
    }

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nЛабораторная работа 11, вариант 12");
            Console.WriteLine("1. Задание 1");
            Console.WriteLine("2. Задание 2");
            Console.WriteLine("3. Задание 3");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            if (choice == "1") Task1();
            else if (choice == "2") Task2();
            else if (choice == "3") Task3();
            else if (choice == "0") break;
            else Console.WriteLine("Нет такого пункта");
        }
    }
}