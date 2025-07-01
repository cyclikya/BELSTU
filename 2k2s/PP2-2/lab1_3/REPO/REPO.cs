namespace REPO
{
    public interface IRepository : IDisposable            
    {
        List<WSRef> getAllWSRef();                    
        List<Comment> getAllComment();                 
        bool addWSRef(WSRef wsRef);              
        bool addComment(Comment comment);      
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
