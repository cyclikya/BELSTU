using REPO;

namespace DALMSQLX
{
    public class Repository : IRepository
    {
        Context context;
        private Repository()
        {
            this.context = new Context();
        }
        public static REPO.IRepository Create() { return new Repository(); }
        public List<REPO.Comment> getAllComment()
        {
            List<REPO.Comment> rc = new List<REPO.Comment>();
            if (this.context.Comments != null)
            {
                rc.AddRange(this.context.Comments);
            }
            return rc;
        }
        public List<REPO.WSRef> getAllWSRef()
        {
            List<REPO.WSRef> rc = new List<REPO.WSRef>();
            if (this.context.WSRefs != null)
            {
                rc.AddRange(this.context.WSRefs);
            }
            return rc;
        }
        public bool addWSRef(REPO.WSRef wsref)
        {
            bool rc = false;
            if (this.context.WSRefs != null)
            {
                context.Database.BeginTransaction();
                context.WSRefs.Add(wsref);
                rc = (context.SaveChanges() > 0);
                REPO.WSRef x = context.WSRefs.OrderByDescending(e => e.Id).First();
                context.Database.CommitTransaction();
            }
            return rc;
        }
        public bool addComment(Comment comment)
        {
            bool rc = false;
            if (context.Comments != null)
            {
                context.Add(comment);
                rc = (context.SaveChanges() > 0);
            }
            return rc;
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
