using System;
using System.Text;

class Rotor
{
    public string Name;
    private string wiring;
    private int position;

    public Rotor(string name, string wiring, int startPosition)
    {
        Name = name;
        this.wiring = wiring;
        position = startPosition;
    }

    public void Step(int step)
    {
        position = (position + step) % 26;
    }

    public int Forward(int c)
    {
        int shifted = (c + position) % 26;
        int encoded = wiring[shifted] - 'A';
        return (encoded - position + 26) % 26;
    }

    public int Backward(int c)
    {
        int shifted = (c + position) % 26;
        int index = wiring.IndexOf((char)(shifted + 'A'));
        return (index - position + 26) % 26;
    }

    public char GetPosition()
    {
        return (char)('A' + position);
    }

    public int Position => position;
}

class Reflector
{
    public string Name;
    private string wiring;

    public Reflector(string name, string wiring)
    {
        Name = name;
        this.wiring = wiring;
    }

    public int Reflect(int c)
    {
        return wiring[c] - 'A';
    }
}

class Enigma
{
    public Rotor L;
    public Rotor M;
    public Rotor R;
    public Reflector reflector;

    public Enigma(int lPos, int mPos, int rPos)
    {
        L = new Rotor("III", "BDFHJLCPRTXVZNYEIWGAKMUSQO", lPos);
        M = new Rotor("Gamma", "FSOKANUERHMBTIYCWLQPZXVGJD", mPos);
        R = new Rotor("V", "VZBRGITYUPSDNHLXAWMJQOFECK", rPos);

        reflector = new Reflector(
            "C Dünn",
            "RDOJNTKVIMLEABPWZXYSUCFGQH"
        );
    }

    public char Encode(char ch)
    {
        if (ch < 'A' || ch > 'Z')
            return ch;

        int c = ch - 'A';

        Console.WriteLine("Вход: " + ch);

        c = R.Forward(c);
        c = M.Forward(c);
        c = L.Forward(c);

        c = reflector.Reflect(c);

        c = L.Backward(c);
        c = M.Backward(c);
        c = R.Backward(c);

        StepRotors();

        char result = (char)(c + 'A');

        Console.WriteLine("Выход: " + result);
        PrintRotorPositions();

        return result;
    }

    private void StepRotors()
    {
        R.Step(2);
        M.Step(1);
        L.Step(1);
    }

    public void PrintConfiguration()
    {
        Console.WriteLine("----- КОНФИГУРАЦИЯ ENIGMA -----");

        Console.WriteLine("Левый ротор  : " + L.Name);
        Console.WriteLine("Средний ротор: " + M.Name);
        Console.WriteLine("Правый ротор : " + R.Name);
        Console.WriteLine("Рефлектор    : " + reflector.Name);

        Console.WriteLine("\nНачальные позиции роторов:");
        PrintRotorPositions();

        Console.WriteLine("--------------------------------\n");
    }

    public void PrintRotorPositions()
    {
        Console.WriteLine(
            "Позиции роторов -> L:" + L.GetPosition() +
            " M:" + M.GetPosition() +
            " R:" + R.GetPosition()
        );
        Console.WriteLine();
    }

    public string EncodeMessage(string text)
    {
        text = text.ToUpper();
        StringBuilder result = new StringBuilder();

        foreach (char c in text)
        {
            result.Append(Encode(c));
        }

        return result.ToString();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("===== СИМУЛЯТОР ENIGMA =====\n");

        Console.Write("Начальная позиция L (A-Z): ");
        int l = Console.ReadLine().ToUpper()[0] - 'A';

        Console.Write("Начальная позиция M (A-Z): ");
        int m = Console.ReadLine().ToUpper()[0] - 'A';

        Console.Write("Начальная позиция R (A-Z): ");
        int r = Console.ReadLine().ToUpper()[0] - 'A';

        Enigma enigma = new Enigma(l, m, r);

        enigma.PrintConfiguration();

        while (true)
        {
            Console.WriteLine("Введите сообщение (или 'exit'):");
            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
                break;

            string encrypted = enigma.EncodeMessage(input);

            Console.WriteLine("\nРезультат шифрования:");
            Console.WriteLine(encrypted);
            Console.WriteLine("\n=============================\n");
        }
    }
}