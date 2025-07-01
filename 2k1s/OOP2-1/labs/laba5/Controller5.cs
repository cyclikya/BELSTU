using System.Numerics;

namespace Laba5
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
            return countries;
        }

        public int CountSeas()
        {
            var seasCount = planet.GetAll().OfType<Sea>().Count();

            return seasCount;
        }

        public void GetIslandsAlphabetically()
        {
            var islands = planet.GetAll()
                             .OfType<Island>() 
                             .OrderBy(island => island.Name)
                             .ToList();

            Console.WriteLine("Острова (по алфавиту):");
            foreach (var island in islands)
            {
                Console.WriteLine($"- {island.Name}");
            }
        }
    }
}
