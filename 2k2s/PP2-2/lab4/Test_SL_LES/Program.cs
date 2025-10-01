using DAL_LES;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator;

class Program
{
    static void Main()
    {
   
        TransientLocator.Register<IRepository>(() => new Repository());
        TransientLocator.Register<ICelebrity>(() => new Repository());
        TransientLocator.Register<ILifeevent>(() => new Repository());
        TransientLocator.Register<ICommon>(() => new Repository());

        Console.WriteLine("Тест TransientLocator\n");

        int celebId;
        int eventId;

        // --- ТЕСТ ICelebrity ---
        using (ICelebrity celebrityService = TransientLocator.Resolve<ICelebrity>())
        {
            Console.WriteLine("ICelebrity: Добавление");

            Celebrity newCelebrity = new Celebrity
            {
                FullName = "Grace Hopper",
                Nationality = "US",
                ReqPhotoPath = "/img/hopper.jpg"
            };

            celebrityService.AddCelebrity(newCelebrity);

            List<Celebrity> celebrities = celebrityService.GetAllCelebrities();
            foreach (Celebrity c in celebrities)
            {
                Console.WriteLine($"[Celebrity] {c.Id}: {c.FullName}, {c.Nationality}");
            }

            celebId = celebrities.First().Id;

            Celebrity loaded = celebrityService.GetCelebrityById(celebId);
            Console.WriteLine($"Загружено по ID: {loaded.FullName}");

            Celebrity updated = new Celebrity
            {
                FullName = "Grace M. Hopper",
                Nationality = "US",
                ReqPhotoPath = "/img/hopper_updated.jpg"
            };

            celebrityService.UpdCelebrity(celebId, updated);
            Console.WriteLine("Обновлено.");
        }

        Console.WriteLine();

        // --- ТЕСТ ILifeevent ---
        using (ILifeevent lifeeventService = TransientLocator.Resolve<ILifeevent>())
        {
            Console.WriteLine("ILifeevent: Добавление");

            Lifeevent lifeevent = new Lifeevent
            {
                CelebrityId = celebId,
                Date = new DateTime(1952, 1, 1),
                Description = "Created first compiler",
                ReqPhotoPath = "/img/compiler.jpg"
            };

            lifeeventService.AddLifeevent(lifeevent);

            List<Lifeevent> events = lifeeventService.GetAllLifeevents();
            foreach (Lifeevent e in events)
            {
                Console.WriteLine($"[Lifeevent] {e.Id}: {e.Date.ToShortDateString()} — {e.Description}");
            }

            eventId = events.First().Id;

            Lifeevent loadedEvent = lifeeventService.GetLifeeventById(eventId);
            Console.WriteLine($"Загружено событие: {loadedEvent.Description}");

            Lifeevent updatedEvent = new Lifeevent
            {
                Id = eventId,
                CelebrityId = celebId,
                Date = new DateTime(1954, 1, 1),
                Description = "Promoted COBOL adoption",
                ReqPhotoPath = "/img/cobol.jpg"
            };

            lifeeventService.UpdLifeevent(eventId, updatedEvent);
            Console.WriteLine("Событие обновлено.");
        }

        Console.WriteLine();

        // --- ТЕСТ ICommon ---
        using (ICommon commonService = TransientLocator.Resolve<ICommon>())
        {
            Console.WriteLine("ICommon: связи между таблицами");

            List<Lifeevent> events = commonService.GetLifeeventsByCelebrityId(celebId);
            foreach (Lifeevent e in events)
            {
                Console.WriteLine($"[ByCelebrityId] {e.Description}");
            }

            Celebrity celebrity = commonService.GetCelebrityByLifeeventId(eventId);
            Console.WriteLine($"[ByEventId] Событие принадлежит: {celebrity.FullName}");
        }

        Console.WriteLine();

        // --- ТЕСТ IRepository: удаление ---
        using (IRepository repo = TransientLocator.Resolve<IRepository>())
        {
            Console.WriteLine("IRepository: Удаление");

            repo.DelLifeevent(eventId);
            Console.WriteLine("Событие удалено.");

            repo.DelCelebrity(celebId);
            Console.WriteLine("Знаменитость удалена.");
        }

        Console.WriteLine("\nКонец.");

        Console.WriteLine("\nТестирование кэша через ScopeLocator");

        // Регистрируем Scoped-сервисы
        ScopeLocator.Register<IRepository>(sp => new Repository());
        ScopeLocator.Register<ICacheCelebrities>(sp =>
        {
            IRepository repo = sp.GetService<IRepository>();
            return new CacheCelebrities(repo, TimeSpan.FromSeconds(5));
        });

        // --- Первый Scope ---
        Console.WriteLine("\n[Scope 1]");
        using (IServiceScope scope1 = ScopeLocator.BeginScope())
        {
            ICacheCelebrities cache1 = scope1.ServiceProvider.GetService<ICacheCelebrities>();

            Console.WriteLine("Первый вызов:");
            List<Celebrity> result1 = cache1.GetCachedCelebrities();
            Console.WriteLine($"Кол-во: {result1.Count}");

            Console.WriteLine("Второй вызов через 2 сек (должен использовать кэш):");
            Thread.Sleep(2000);
            List<Celebrity> result2 = cache1.GetCachedCelebrities();
            Console.WriteLine($"Кол-во: {result2.Count}");

            Console.WriteLine("Третий вызов через 5 сек (должен обновить кэш):");
            Thread.Sleep(5000);
            List<Celebrity> result3 = cache1.GetCachedCelebrities();
            Console.WriteLine($"Кол-во: {result3.Count}");
        }

        // --- Второй Scope ---
        Console.WriteLine("\n[Scope 2] — новый объект кэша");
        using (IServiceScope scope2 = ScopeLocator.BeginScope())
        {
            ICacheCelebrities cache2 = scope2.ServiceProvider.GetService<ICacheCelebrities>();

            Console.WriteLine("Первый вызов (новый кэш):");
            List<Celebrity> result4 = cache2.GetCachedCelebrities();
            Console.WriteLine($"Кол-во: {result4.Count}");
        }

    }
}
