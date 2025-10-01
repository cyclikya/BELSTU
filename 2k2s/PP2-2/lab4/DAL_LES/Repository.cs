using Microsoft.EntityFrameworkCore;

namespace DAL_LES
{
    public interface IRepository : ICommon, ICelebrity, ILifeevent, IDisposable
    { 
        Guid id { get; }
    }

    public class Repository : IRepository, IDisposable
    {
        private readonly Context _context;

        public Repository()
        {
            _context = new Context();
            _context.Database.EnsureCreated();
        }

        public List<Celebrity> GetAllCelebrities() => _context.Celebrities.ToList();

        public Celebrity GetCelebrityById(int id) => _context.Celebrities.Find(id);

        public bool AddCelebrity(Celebrity celebrity)
        {
            _context.Celebrities.Add(celebrity);
            return _context.SaveChanges() > 0;
        }

        public bool UpdCelebrity(int id, Celebrity updated)
        {
            Celebrity? existing = _context.Celebrities.Find(id);
            if (existing == null) return false;

            existing.FullName = updated.FullName;
            existing.Nationality = updated.Nationality;
            existing.ReqPhotoPath = updated.ReqPhotoPath;

            return _context.SaveChanges() > 0;
        }

        public bool DelCelebrity(int id)
        {
            Celebrity? celebrity = _context.Celebrities.Find(id);
            if (celebrity == null) return false;

            _context.Celebrities.Remove(celebrity);
            return _context.SaveChanges() > 0;
        }

        public List<Lifeevent> GetAllLifeevents() => _context.Lifeevents.ToList();

        public Lifeevent GetLifeeventById(int id) => _context.Lifeevents.Find(id);

        public bool AddLifeevent(Lifeevent lifeevent)
        {
            _context.Lifeevents.Add(lifeevent);
            return _context.SaveChanges() > 0;
        }

        public bool UpdLifeevent(int id, Lifeevent updated)
        {
            Lifeevent? existing = _context.Lifeevents.Find(id);
            if (existing == null) return false;

            existing.Description = updated.Description;
            existing.Date = updated.Date;
            existing.ReqPhotoPath = updated.ReqPhotoPath;
            existing.CelebrityId = updated.CelebrityId;

            return _context.SaveChanges() > 0;
        }

        public bool DelLifeevent(int id)
        {
            Lifeevent? evt = _context.Lifeevents.Find(id);
            if (evt == null) return false;

            _context.Lifeevents.Remove(evt);
            return _context.SaveChanges() > 0;
        }

        public List<Lifeevent> GetLifeeventsByCelebrityId(int celebrityId)
        {
            return _context.Lifeevents
                .Where(e => e.CelebrityId == celebrityId)
                .ToList();
        }

        public Celebrity? GetCelebrityByLifeeventId(int lifeeventId)
        {
            Lifeevent? evt = _context.Lifeevents
                .Include(e => e.Celebrity)
                .FirstOrDefault(e => e.Id == lifeeventId);

            return evt?.Celebrity;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        Guid id { get; } = Guid.NewGuid();

        Guid IRepository.id => id;

        public static IRepository Create()
        {
            return new Repository();
        }
    }
}
