using System;

public abstract class Mainland : IVisit
{
    public string climate;

    public string Climate
    {
        get { return climate; }
        set { climate = value; }
    }
    public virtual string DoVisit()
    {
        return $"Вы посетили что-то на суше";
    }
    public abstract override string ToString();

}