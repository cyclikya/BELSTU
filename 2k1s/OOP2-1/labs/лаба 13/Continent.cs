using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Continent : Mainland, IVisit
{
    protected string contName;
    protected string contArea;

    protected List<Country> countries;

    public string ContName
    {
        get { return contName; }
        set { contName = value; }
    }

    public string ContArea
    {
        get { return contArea; }
        set { contArea = value; }
    }

    public Continent(string contName, string contArea, string climate)
    {
        ContName = contName;
        ContArea = contArea;
        Climate = climate;
    }


    public void AddCountry(Country country)
    {
        this.countries.Add(country);
    }

    public Country this[int index]
    {
        get { return countries[index]; }
    }

    public override string ToString()
    {
        return $"{GetType()}: Континент: {contName}, площадь: {contArea}, климат: {climate}";
    }
    public override string DoVisit()
    {
        return $"Вы посетили континент {contName}";
    }

    string IVisit.DoVisit()
    {
        return $"Вы посетили континент {contName} через интерфейс";
    }
}

