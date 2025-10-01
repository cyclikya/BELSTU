using laba6;
using System;
using System.Diagnostics;

namespace Laba6
{
    public enum TerrainCategory 
    {
        Water,
        Land,
        Mixed
    }

    public struct Coordinates
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinates(string latitude, string longitude)
        {
            double latitudeDouble;
            double longitudeDouble;

            try
            {
                latitudeDouble = double.Parse(latitude);
                longitudeDouble = double.Parse(longitude);
            }
            catch (FormatException)
            {
                throw new InvalidFormatCoordinatesException($"Ошибка ввода: координаты должны быть числами. Вы ввели: {latitude} и {longitude}");
            }

            if (latitudeDouble < -90 || latitudeDouble > 90 || longitudeDouble < -180 || longitudeDouble > 180)
            {
                throw new InvalidCoordinatesException($"Координаты {latitude} и {longitude} выходят за допустимые пределы.");
            }

            Latitude = latitudeDouble;
            Longitude = longitudeDouble;
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

        public void Add(EarthObject obj) //assert
        {
            Debug.Assert(!objects.Any(o => o.Name == obj.Name), "Объект с таким именем уже существует!");
            objects.Add(obj);
        }

        public void Remove(EarthObject obj)
        {
            if (!objects.Contains(obj))
            {
                throw new KeyNotFoundException($"Объект {obj.Name} не найден для удаления.");
            }
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
            EarthController controller = new EarthController(planet);
            try { 
                try
                {
                    // Добавляем объекты
                    planet.Add(new Country("France", "Europe"));
                    var countries1 = controller.FindCountriesByContinent("Europe");

                    //var countries2 = controller.FindCountriesByContinent("Asia");

                    //Console.WriteLine($"Количество морей: {controller.CountSeas()}");

                    planet.Add(new Sea("Mediterranean Sea", new Coordinates("35,0", "18,0")));
                    Console.WriteLine($"Количество морей: {controller.CountSeas()}");

                    //Sea sea0 = new Sea("Red sea", new Coordinates("35,0", "18,0"));
                    //planet.Remove(sea0);

                    //controller.GetIslandsAlphabetically();

                    planet.Add(new Island("Greenland"));
                    planet.Add(new Island("Abadan"));
                    controller.GetIslandsAlphabetically();

                    Sea sea1 = new Sea("Mediterranean Sea", new Coordinates("-91", "18"));
                    //Sea sea2 = new Sea("Mediterranean Sea", new Coordinates("abc", "18"));

                    //planet.Add(new Country("France", "Europe"));
                }
                catch (InvalidCoordinatesException ex)
                {
                    Console.WriteLine($"Неверные координаты: {ex.Message}");
                        throw;
                }
                catch (InvalidFormatCoordinatesException ex)
                {
                    Console.WriteLine($"Ошибка формата: {ex.Message}");
                        throw;
                }
                finally
                {
                    Console.WriteLine("Завершение обработки.");
                }
            }
            catch (ERROR ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message} (Тип ошибки: {ex.GetType().Name})");
            }
        }
    }
}
