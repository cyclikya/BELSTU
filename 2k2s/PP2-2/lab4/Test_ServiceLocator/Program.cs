using Microsoft.Extensions.DependencyInjection;
using ServiceLocator;
using DAL_LES;

class Program
{
    static void Main()
    {
        TransientLocator.Register<IRepository>(() => Repository.Create());

        using (IRepository repo = TransientLocator.Resolve<IRepository>()) // Каждый вызов Resolve создает новый экземпляр сервиса.
        {
            Console.WriteLine($"Transient: {repo.id}");
        }

        using (IRepository repo = TransientLocator.Resolve<IRepository>())
        {
            Console.WriteLine($"Transient: {repo.id}");
        }

        Console.WriteLine();

        SingletonLocator.Register<IRepository>(Repository.Create()); // Создается один экземпляр сервиса, который используется повторно для всех запросов.

        using (IRepository repo = SingletonLocator.Resolve<IRepository>())
        {
            Console.WriteLine($"Singleton: {repo.id}");
        }

        using (IRepository repo = SingletonLocator.Resolve<IRepository>())
        {
            Console.WriteLine($"Singleton: {repo.id}");
        }

        Console.WriteLine();

        ScopeLocator.Register<IRepository>(sp => Repository.Create());

        using (IServiceScope scope1 = ScopeLocator.BeginScope())
        {
            IRepository repo1 = ScopeLocator.Resolve<IRepository>();
            IRepository repo2 = ScopeLocator.Resolve<IRepository>();
            Console.WriteLine($"Scope 1 - Repo 1: {repo1.id}");
            Console.WriteLine($"Scope 1 - Repo 2: {repo2.id}");
        }

        using (IServiceScope scope2 = ScopeLocator.BeginScope())
        {
            IRepository repo3 = ScopeLocator.Resolve<IRepository>();
            Console.WriteLine($"Scope 2 - Repo 1: {repo3.id}");
        }
    }
}