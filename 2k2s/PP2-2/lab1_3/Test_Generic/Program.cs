using DALMSQLXG;
using DALJSONXG;
using GREPO;
using DALMSQLX;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("------>Start DALMSQLXG \n");
        DALMSQLX.Init.Execute();
        using (DALMSQLXG.IRepository repo = DALMSQLXG.Repository.Create())
        {
            RunTests(repo);
            RunTests(repo);
        }

        Console.WriteLine("\n\n------>Start DALJSONXG \n");
        DALJSONXG.Init.Execute();
        using (DALJSONXG.IRepository repo = DALJSONXG.Repository.Create())
        {
            RunTests(repo);
            RunTests(repo);
        }

        Console.WriteLine("Finish");
        Console.ReadLine();
    }

    private static void RunTests(GREPO.IRepository repo)
    {
        repo.getAllWSRef().ForEach(wsRef =>
        {
            Console.WriteLine($"WSRefs: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
        });

        repo.getAllComment().ForEach(comment =>
        {
            Console.WriteLine($"Comments {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSRefId}");
        });

        if (repo.addWSRef(new WSRef() { Url = "https://www.belstu.by/", Description = "БГТУ", Minus = 0, Plus = 1 }))
            Console.WriteLine("WSRefs: Add");
        else
            Console.WriteLine("WSRefs: Error Add");

        if (repo.addComment(new Comment() { WSRefId = 3, Commtext = "test", Stamp = DateTime.Now }))
            Console.WriteLine("Comments: Add");
        else
            Console.WriteLine("Comments: Error Add");

        Console.WriteLine("After addWSRef, addComment");

        repo.getAllWSRef().ForEach(wsRef =>
        {
            Console.WriteLine($"WSRefs: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
        });

        repo.getAllComment().ForEach(comment =>
        {
            Console.WriteLine($"Comments {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSRefId}");
        });

        Console.WriteLine("-----------------------------------------------------------------");
    }
}
