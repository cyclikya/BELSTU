using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_Celebrity_Npgsql
{
    public interface IRepository : DAL_Celebrity.IRepository<Celebrity, Lifeevent> { }

    public class Repository : IRepository
    {
        Context context;
        public Repository() { this.context = new Context(); }
        public Repository(string connectionstring) { this.context = new Context(connectionstring); }
        public static IRepository Create() { return new Repository(); }
        public static IRepository Create(string connectionstring) { return new Repository(connectionstring); }
        public List<Celebrity> GetAllCelebrities() { return this.context.Celebrities.ToList<Celebrity>(); }
        public Celebrity? GetCelebrityById(int Id)
        {
            return this.context.Celebrities.FirstOrDefault(c => c.Id == Id);
        }
        public bool AddCelebrity(Celebrity celebrity)
        {
            this.context.Celebrities.Add(celebrity);
            return this.context.SaveChanges() > 0;
        }

        public bool DelCelebrity(int id)
        {
            var cel = this.context.Celebrities.FirstOrDefault(c => c.Id == id);
            if (cel != null)
            {
                this.context.Celebrities.Remove(cel);
                return this.context.SaveChanges() > 0;
            }
            return false;
        }

        public bool? UpdCelebrity(int id, Celebrity celebrity)
        {
            var cel = this.context.Celebrities.FirstOrDefault(c => c.Id == id);
            if (cel != null)
            {
                cel.FullName = celebrity.FullName;
                cel.Nationality = celebrity.Nationality;
                cel.ReqPhotoPath = celebrity.ReqPhotoPath;
                return this.context.SaveChanges() > 0;
            }
            return false;
        }
        public List<Lifeevent> GetAllLifeevents() { return this.context.Lifeevents.ToList<Lifeevent>(); }
        public Lifeevent? GetLifeeventById(int Id)
        {
            return this.context.Lifeevents.FirstOrDefault(c => c.Id == Id);
        }
        public bool AddLifeevent(Lifeevent lifeevent)
        {
            this.context.Lifeevents.Add(lifeevent);
            return this.context.SaveChanges() > 0;
        }
        public bool DelLifeevent(int id)
        {
            var l = this.context.Lifeevents.FirstOrDefault(c => c.Id == id);
            if (l != null)
            {
                this.context.Lifeevents.Remove(l);
                return this.context.SaveChanges() > 0;
            }
            return false;
        }
        public bool UpdLifeevent(int id, Lifeevent lifeevent)
        {
            var l = this.context.Lifeevents.FirstOrDefault(c => c.Id == id);
            if (l != null)
            {
                l.CelebrityId = lifeevent.CelebrityId;
                l.ReqPhotoPath = lifeevent.ReqPhotoPath;
                l.Description = lifeevent.Description;
                l.Date = lifeevent.Date;
                return this.context.SaveChanges() > 0;
            }
            return false;
        }
        public List<Lifeevent> GetLifeeventsByCelebrityId(int celebrityId)
        {
            return this.context.Lifeevents.Where(p => p.CelebrityId == celebrityId).ToList();
        }
        public Celebrity? GetCelebrityByLifeeventId(int lifeeventId)
        {
            var l = this.context.Lifeevents.FirstOrDefault(p => p.Id == lifeeventId);
            return this.context.Celebrities.FirstOrDefault(p => p.Id == l.CelebrityId);
        }
        public int GetCelebrityIdByName(string name)
        {
            return this.context.Celebrities.FirstOrDefault(p => p.FullName.Contains(name)).Id;
        }
        public void Dispose() { }
    }
}
