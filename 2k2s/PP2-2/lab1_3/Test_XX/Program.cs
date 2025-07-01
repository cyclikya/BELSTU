using DALMSQLX;
using DALJSONX;
using REPO;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("------>Start DALMSQLX \n");
        DALMSQLX.Init.Execute();
        Console.WriteLine("\n------>AfterExecute");


        using (REPO.IRepository repo = DALMSQLX.Repository.Create())
        {
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSRefId} ");
            });

            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });
            repo.addWSRef(new REPO.WSRef() { Url = "https://www.belstu.by/", Description = "БГТУ", Minus = 0, Plus = 1 });
            Console.WriteLine("-----------------------------------------------------------------");
            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSRefId} ");
            });
        }
        Console.WriteLine("-----------------------------------------------------------------");



        Console.WriteLine("\n\nInit");
        DALJSONX.Init.Execute();                           

        Console.WriteLine("Start DALJSONX");
        using (REPO.IRepository repo = DALJSONX.Repository.Create())
        {
            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"WSRefs: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });

            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"Comments {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSRefId} ");
            });

            if (repo.addWSRef(new REPO.WSRef() { Url = "https://www.belstu.by/", Description = "БГТУ", Minus = 0, Plus = 0 })) Console.WriteLine("WSRefs: Add");
            else Console.WriteLine("WSRefs: Error Add");

            if (repo.addComment(new REPO.Comment() { WSRefId = 3, Commtext = "test", Stamp = DateTime.Now })) Console.WriteLine("Comments: Add");
            else Console.WriteLine("Comments: Error Add");

            Console.WriteLine("After addWSRef, addComment ");

            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"WSRefs: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });

            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"Comments {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSRefId} ");
            });

        }
        Console.WriteLine("Finish");
        Console.ReadLine();
    }
}