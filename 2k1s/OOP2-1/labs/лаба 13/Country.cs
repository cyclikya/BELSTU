using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Country : Continent, IVisit
{
    private string countryName;
    private string countryArea;
    private string capital;
    private int population;


    public string CountryName
    {
        get { return countryName; }
        set { countryName = value; }
    }

    public string CountryArea
    {
        get { return countryArea; }
        set { countryArea = value; }
    }

    public string Capital
    {
        get { return capital; }
        set { capital = value; }
    }

    public int Population
    {
        get { return population; }
        set { population = value; }
    }

    public Country(string countryName, string countryArea, string capital, int population, string contName, string contArea, string climate) : base(contName, contArea, climate)
    {
        CountryName = countryName;
        CountryArea = countryArea;
        Capital = capital;
        Population = population;
        Climate = climate;
    }

    public override string ToString()
    {
        return $"{GetType()}: Название страны: {countryName}, площадь: {countryArea}, столица: {capital}, население: {population}, климат: {climate}";
    }
    public override string DoVisit()
    {
        return $"Вы посетили страну {countryName} на контененте {contName}";
    }

    string IVisit.DoVisit()
    {
        return $"Вы посетили страну {countryName}";
    }

}
