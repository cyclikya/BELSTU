using System;

public interface IDescribe
{
    void Describe();
}
public interface ICloneable
{
    bool DoClone();
}
public abstract class Water : IDescribe //класс abstract
{
    public string Name;

    public Water(string name)
    {
        Name = name;
    }
    public abstract bool DoClone();

    public virtual void Describe() //метод virtual
    {
        Console.WriteLine($"Это вода под названием {Name}.");
    }

    public override string ToString()
    {
        return $"Тип: {this.GetType().Name}, Название: {Name}";
    }
}

public class Land : IDescribe
{
    public string Name;

    public Land(string name)
    {
        Name = name;
    }

    public virtual void Describe() //метод virtual
    {
        Console.WriteLine($"Это земля под названием {Name}.");
    }
    public override string ToString()
    {
        return $"Тип: {this.GetType().Name}, Название: {Name}";
    }
}

public class Sea : Water, ICloneable
{
    public string Location;

    public Sea(string name, string location) : base(name)
    {
        Location = location;
    }
    public override bool DoClone()
    {
        Sea cloned = new Sea(this.Name, this.Location);
        Console.WriteLine("Через Base");
        return true;
    }
    bool ICloneable.DoClone()
    {
        Console.WriteLine("Через Interface");
        return true;
    }
    public override void Describe()
    {
        Console.WriteLine($"Море {Name} расположено в {Location}.");
    }

    public override bool Equals(object obj)
    {
        if (obj is Sea)
        {
            Sea other = (Sea)obj;
            return this.Name == other.Name && this.Location == other.Location;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public Sea Clone()
    {
        return new Sea(this.Name, this.Location);
    }

    public override string ToString()
    {
        return $"Тип: {this.GetType().Name}, Название: {Name}, Расположение: {Location}";
    }
}

public class Continent : Land
{
    public Continent(string name) : base(name) { }

    public void Display()
    {
        Console.WriteLine($"Континент: {Name}");
    }
    public override string ToString()
    {
        return $"Тип: {this.GetType().Name}, Название: {Name}";
    }
}

public sealed class Country : Land //класс sealed
{
    public string Capital;

    public Country(string name, string capital) : base(name)
    {
        Capital = capital;
    }
    public override void Describe()
    {
        Console.WriteLine($"Страна: {Name}, Столица: {Capital}");
    }
    public override string ToString()
    {
        return $"Тип: {this.GetType().Name}, Название: {Name}, Столица: {Capital}";
    }
}

public class Island : Land
{
    public Sea SurroundedBySea;

    public Island(string name, Sea sea) : base(name)
    {
        SurroundedBySea = sea;
    }

    public override void Describe()
    {
        Console.WriteLine($"Остров: {Name}, Окружено: {SurroundedBySea.Name}");
    }
    public override string ToString()
    {
        return $"Тип: {this.GetType().Name}, Название: {Name}, Окружено: {SurroundedBySea.Name}";
    }
}

public class Printer
{
    public void IAmPrinting(IDescribe obj)
    {
        Console.WriteLine($"Тип объекта: {obj.GetType().Name}, Детали ( {obj.ToString()})");
    }
}

class LR4
{
    static void Main()
    {
        Sea blackSea = new Sea("Черное море", "Восточное европа");
        IDescribe asia = new Continent("Азия");
        IDescribe russia = new Country("Россия", "Москва");
        IDescribe iJapan = new Island("Япония", (Sea)blackSea);

        blackSea.Describe();
        asia.Describe();
        russia.Describe();
        iJapan.Describe();

        Console.WriteLine(blackSea.ToString());
        Console.WriteLine(asia.ToString());
        Console.WriteLine(russia.ToString());
        Console.WriteLine(iJapan.ToString());

        if (blackSea is Sea sea)
        {
            Console.WriteLine("\n" + sea.ToString());
        }

        IDescribe continent = asia as Continent; //он будет на него ссылаться
        if (continent != null)
        {
            continent.Describe();
        }

        Sea norvSea = new Sea("Норвежское море", "Северная европа");
        Console.WriteLine(blackSea.Equals(norvSea));
        Console.WriteLine(blackSea.Equals(blackSea));

        Sea clonedSea = ((Sea)blackSea).Clone();
        Console.WriteLine($"Клонированное море: {clonedSea.Name}");


        Printer printer = new Printer();

        IDescribe[] objects = { blackSea, asia, russia, iJapan };

        Console.WriteLine("\nВывод массива объектов:");
        foreach (var obj in objects)
        {
            printer.IAmPrinting(obj);
        }


        bool resultByBase = blackSea.DoClone();

        ICloneable cloneableSea = new Sea("sdfg", "sdfg");
        bool ResultByInr = cloneableSea.DoClone();
    }
}
