namespace DAL_LES
{
    public interface ICelebrity<T> : IDisposable
    {
        List<T> GetAllCelebrities();
        T GetCelebrityById(int id);
        bool DelCelebrity(int id);
        bool AddCelebrity(T celebrity);
        bool UpdCelebrity(int id, T celebrity);
    }

    public interface ICelebrity : ICelebrity<Celebrity> { }
}
