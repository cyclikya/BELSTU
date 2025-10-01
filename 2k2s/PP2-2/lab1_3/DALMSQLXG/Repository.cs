using GREPO;

namespace DALMSQLXG
{
    public interface IRepository : GREPO.IRepository { }
    public class Repository : IRepository 
    {
        Context context;
        private Repository()
        {
            this.context = new Context();
        }
        public static IRepository Create() { return new Repository(); }
        public List<Comment> getAllComment()
        {
            List<Comment> rc = new List<Comment>();
            if (this.context.Comments != null)
            {
                rc.AddRange(this.context.Comments);
            }
            return rc;
        }
        public List<WSRef> getAllWSRef()
        {
            List<WSRef> rc = new List<WSRef>();
            if (this.context.WSRefs != null)
            {
                rc.AddRange(this.context.WSRefs);
            }
            return rc;
        }
        public bool addWSRef(WSRef wsref)
        {
            bool rc = false;
            if (this.context.WSRefs != null)
            {
                context.Database.BeginTransaction();
                context.WSRefs.Add(wsref);
                rc = (context.SaveChanges() > 0);
                WSRef x = context.WSRefs.OrderByDescending(e => e.Id).First();
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

        public Comment? GetCommentById(int id)
        {
            var comment = this.context.Comments.FirstOrDefault(c => c.Id == id);
            return comment;
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
