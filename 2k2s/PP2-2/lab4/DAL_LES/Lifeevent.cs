namespace DAL_LES
{
    public class Lifeevent
    {
        public int Id { get; set; }
        public int CelebrityId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReqPhotoPath { get; set; } = string.Empty;

        public Celebrity? Celebrity { get; set; }
    }

}
