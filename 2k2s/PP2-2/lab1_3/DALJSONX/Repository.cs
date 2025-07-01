﻿using REPO;

namespace DALJSONX
{
    public class Repository : REPO.IRepository
    {
        JSONContext context;
        private Repository()
        {
            this.context = JSONContext.Create("WSRefX.json");
        }
        public static IRepository Create()
        {
            return new Repository();
        }
        public List<Comment> getAllComment()
        {
            return this.context.Comments == null ? new List<Comment>() : this.context.Comments;
        }
        public List<WSRef> getAllWSRef()
        {
            return this.context.WSRefs == null ? new List<WSRef>() : this.context.WSRefs;
        }
        public bool addWSRef(WSRef wsref)
        {
            bool rc = false;
            if (this.context.addWSRef(wsref)) rc = (this.context.SaveChanges() > 0);
            return rc;
        }
        public bool addComment(Comment comment)
        {
            bool rc = false;
            if (this.context.addComment(comment)) rc = (context.SaveChanges() > 0);
            return rc;
        }
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
