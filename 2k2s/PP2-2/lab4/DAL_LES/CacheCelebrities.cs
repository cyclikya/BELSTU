namespace DAL_LES
{
    public interface ICacheCelebrities
    {
        List<Celebrity> GetCachedCelebrities();
    }

    public class CacheCelebrities : ICacheCelebrities, IDisposable
    {
        private readonly IRepository _repo;
        private List<Celebrity> _cachedCelebrities;
        private DateTime _lastUpdated;
        private readonly TimeSpan _cacheLifetime;

        public CacheCelebrities(IRepository repo, TimeSpan cacheLifetime)
        {
            _repo = repo;
            _cacheLifetime = cacheLifetime;
            _cachedCelebrities = new List<Celebrity>();
            _lastUpdated = DateTime.MinValue;
        }

        public List<Celebrity> GetCachedCelebrities()
        {
            if (DateTime.Now - _lastUpdated > _cacheLifetime)
            {
                Console.WriteLine($"Обновляем кэш.");
                _cachedCelebrities = _repo.GetAllCelebrities();
                _lastUpdated = DateTime.Now;
            }
            else
            {
                Console.WriteLine($"В данный момент кэш актуален.");
            }

            return _cachedCelebrities;
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}
