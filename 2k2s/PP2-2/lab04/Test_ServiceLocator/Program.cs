﻿using Microsoft.Extensions.DependencyInjection;
using ServiceLocator;
using DAL_LES;

class Program
{
    static void Main()
    {
        TestSingleton();
        TestTransient();
        TestScope();
    }

    static void TestSingleton()
    {
        Guid id1;
        Guid id2;

        SingletonLocator.Register<IRepository>(Repository.Create());

        IRepository repo = SingletonLocator.Resolve<IRepository>();
        id1 = repo.Id;
        Console.WriteLine($"Singleton: {id1}");

        IRepository repo2 = SingletonLocator.Resolve<IRepository>();
        id2 = repo2.Id;
        Console.WriteLine($"Singleton: {id2}\n");
    }

    static void TestTransient()
    {
        Guid id1;
        Guid id2;

        TransientLocator.Register<IRepository>(() => Repository.Create());

        IRepository repo = TransientLocator.Resolve<IRepository>();
        id1 = repo.Id;
        Console.WriteLine($"Transient: {id1}");

        IRepository repo2 = TransientLocator.Resolve<IRepository>();
        id2 = repo2.Id;
        Console.WriteLine($"Transient: {id2}\n");
    }

    static void TestScope()
    {
        Guid scope1a;
        Guid scope1b;
        Guid scope2a;

        ScopeLocator.Register<IRepository>(() => Repository.Create());

        using (var scope1 = ScopeLocator.CreateServiceScopeFactory().CreateScope())
        {
            IRepository repo1a = ScopeLocator.Resolve<IRepository>(scope1);
            IRepository repo1b = ScopeLocator.Resolve<IRepository>(scope1);

            scope1a = repo1a.Id;
            scope1b = repo1b.Id;

            Console.WriteLine($"Scope 1 - Repo 1: {scope1a}");
            Console.WriteLine($"Scope 1 - Repo 2: {scope1b}");
        }

        using (var scope2 = ScopeLocator.CreateServiceScopeFactory().CreateScope())
        {
            IRepository repo2a = ScopeLocator.Resolve<IRepository>(scope2);
            scope2a = repo2a.Id;

            Console.WriteLine($"Scope 2 - Repo 1: {scope2a}");
        }
    }
}