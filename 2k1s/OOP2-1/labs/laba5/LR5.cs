using System;
using System.Collections.Generic;
using System.Linq;  

namespace Laba5
{
    public enum TerrainCategory // Перечисление
    {
        Water,
        Land,
        Mixed
    }

    public struct Coordinates // Структура
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public override string ToString()
        {
            return $"Широта: {Latitude}, Долгота: {Longitude}";
        }
    }

    abstract class EarthObject
    {
        public string Name;
        public EarthObject(string name) 
        {
            Name = name;
        }
    }

    abstract class Land : EarthObject
    {
        public Land(string name) : base(name) { }
    }

    abstract class Water : EarthObject
    {
        public Coordinates LocationCoordinates { get; }

        protected Water(string name, Coordinates coordinates) : base(name)
        {
            LocationCoordinates = coordinates;
        }
    }

    class Country : Land 
    {
        public string Continent;

        public Country(string name, string continent) : base(name)
        {
            Continent = continent;
        }
        public override string ToString()
        {
            return $"Страна: {Name}, Континент: {Continent}";
        } 
    }

    class Island : Land
    {
        public Island(string name) : base(name) { }

        public override string ToString()
        {
            return $"Остров: {Name}";
        } 
    }

    class Sea : Water
    {
        public Sea(string name, Coordinates coordinates) : base(name, coordinates) { }

        public override string ToString()
        {
            return $"Море: {Name}, Координаты: {LocationCoordinates}";
        } 
    }

    class PlanetEarth
    {
        private List<EarthObject> objects = new List<EarthObject>();

        public void Add(EarthObject obj) 
        {
            objects.Add(obj);
        } 

        public void Remove(EarthObject obj) 
        {
            objects.Remove(obj);
        }

        public List<EarthObject> GetAll()
        {
            return new List<EarthObject>(objects);
        }

        public void PrintAll()
        {
            Console.WriteLine("Объекты на Земле:");
            foreach (var obj in objects)
            {
                Console.WriteLine($"- {obj}");
            }
        }
    }
    class Program
    {
        static void Main()
        {
            PlanetEarth planet = new PlanetEarth();

            planet.Add(new Country("France", "Europe"));
            planet.Add(new Country("Germany", "Europe"));
            planet.Add(new Country("Brazil", "South America"));
            planet.Add(new Sea("Mediterranean Sea", new Coordinates(35.0, 18.0)));
            planet.Add(new Sea("Baltic Sea", new Coordinates(56.0, 20.0)));
            planet.Add(new Island("Greenland"));
            planet.Add(new Island("Madagascar"));

            EarthController controller = new EarthController(planet);

            planet.PrintAll();

            Console.WriteLine("\nГосударства в Европе:");
            var countries = controller.FindCountriesByContinent("Europe");
            countries.ForEach(c => Console.WriteLine($"- {c}"));

            Console.WriteLine($"\nКоличество морей: {controller.CountSeas()}");

            controller.GetIslandsAlphabetically();
        }
    }
}
