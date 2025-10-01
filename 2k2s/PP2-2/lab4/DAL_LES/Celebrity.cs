namespace DAL_LES
{
    public class Celebrity
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Nationality { get; set; } = "XX";
        public string ReqPhotoPath { get; set; } = string.Empty;

        public List<Lifeevent> Lifeevents { get; set; } = new();
    }
}
