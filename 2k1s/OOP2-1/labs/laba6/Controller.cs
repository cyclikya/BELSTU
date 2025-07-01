using laba6;
using System.Numerics;

namespace Laba6
{
    partial class EarthController
    {
        private PlanetEarth planet;
        public EarthController(PlanetEarth planet)
        {
            this.planet = planet;
        }

        public List<Country> FindCountriesByContinent(string continent)
        {
            var countries = planet.GetAll().OfType<Country>()
                .Where(c => c.Continent == continent)
                .ToList();

            if (countries.Count == 0)
            {
                throw new ContinentNotFoundException($"Не найдено стран на континенте: {continent}");
            }

            return countries;
        }

        public int CountSeas()
        {
            var seasCount = planet.GetAll().OfType<Sea>().Count();

            if (seasCount == 0)
            {
                throw new NoSeasException("На планете нет морей.");
            }

            return seasCount;
        }

        public void GetIslandsAlphabetically()
        {
            var islands = planet.GetAll()
                             .OfType<Island>() 
                             .OrderBy(island => island.Name)
                             .ToList();

            if (islands.Count == 0)
            {
                throw new NoIslandsException("На планете нет островов.");
            }

            Console.WriteLine("Острова (по алфавиту):");
            foreach (var island in islands)
            {
                Console.WriteLine($"- {island.Name}");
            }
        }
    }
}
