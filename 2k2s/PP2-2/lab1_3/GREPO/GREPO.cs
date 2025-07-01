namespace GREPO
{
    public interface IRepository : IRepository<WSRef, Comment> { }

    public interface IRepository<T1, T2> : IDisposable            
    {
        List<T1> getAllWSRef();                    
        List<T2> getAllComment();  
        T2? GetCommentById(int id);
        bool addWSRef(T1 wsRef);              
        bool addComment(T2 comment);        
    }
    public class WSRef
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public int? Plus { get; set; }
        public int? Minus { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
    public class Comment
    {
        public int? Id { get; set; }
        public DateTime? Stamp { get; set; }
        public string? Commtext { get; set; }
        public int WSRefId { get; set; }
    }
}
