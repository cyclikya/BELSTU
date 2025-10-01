using DAL_LES;
class Program
{
    static void Main()
    {
        using (IRepository repo = new Repository())
        {
            Console.WriteLine("1. Добавляем знаменитость");
            Celebrity newCelebrity = new Celebrity
            {
                FullName = "Alan Turing",
                Nationality = "UK",
                ReqPhotoPath = "/img/turing.jpg"
            };

            repo.AddCelebrity(newCelebrity);
            Console.WriteLine($"Добавлено: {newCelebrity.FullName}\n");

            Console.WriteLine("2. Получаем список всех знаменитостей.");
            List<Celebrity> celebrities = repo.GetAllCelebrities();
            foreach (Celebrity c in celebrities)
            {
                Console.WriteLine($"Id: {c.Id}, FullName: {c.FullName}, Nationality: {c.Nationality}");
            }

            int celebId = celebrities.First().Id;

            Console.WriteLine("\n3. Получаем знаменитость по id.");
            Celebrity? loaded = repo.GetCelebrityById(celebId);
            Console.WriteLine($"Знаменитость по Id {celebId}: {loaded.FullName}");

            Console.WriteLine("\n4. Обновляем знаменитость.");
            Celebrity updated = new Celebrity
            {
                FullName = "Alan M. Turing",
                Nationality = "UK",
                ReqPhotoPath = "/img/turing_updated.jpg"
            };
            repo.UpdCelebrity(celebId, updated);
            Console.WriteLine("Знаменитость обновлена.");

            Console.WriteLine("\n5. Добавляем событие.");
            Lifeevent newEvent = new Lifeevent
            {
                CelebrityId = celebId,
                Date = new DateTime(1936, 1, 1),
                Description = "Published paper on Turing machines",
                ReqPhotoPath = "/img/event1.jpg"
            };

            repo.AddLifeevent(newEvent);
            Console.WriteLine($"Добавлено событие: {newEvent.Description}\n");

            Console.WriteLine("6. Получаем список всех событий.");
            List<Lifeevent> events = repo.GetAllLifeevents();
            foreach (Lifeevent e in events)
            {
                Console.WriteLine($"Id: {e.Id}, Date: {e.Date.ToShortDateString()}, Desc: {e.Description}");
            }

            int eventId = events.First().Id;

            Console.WriteLine("\n7. Получаем событие по id");
            Lifeevent? loadedEvent = repo.GetLifeeventById(eventId);
            Console.WriteLine($"Событие по Id {eventId}: {loadedEvent.Description}");

            Console.WriteLine("\n8. Обновляем событие.");
            Lifeevent updatedEvent = new Lifeevent
            {
                Id = eventId,
                CelebrityId = celebId,
                Date = new DateTime(1945, 1, 1),
                Description = "Worked on cryptography at Bletchley Park",
                ReqPhotoPath = "/img/event_updated.jpg"
            };

            repo.UpdLifeevent(eventId, updatedEvent);
            Console.WriteLine("Событие обновлено.");

            Console.WriteLine("\n9. Получаем знаменитость по id события.");
            Celebrity? fromEvent = repo.GetCelebrityByLifeeventId(eventId);
            Console.WriteLine($"Знаменитость по событию {eventId}: {fromEvent.FullName}");

            Console.WriteLine("\n10. Удаляем событие.");
            repo.DelLifeevent(eventId);
            Console.WriteLine("Событие удалено.");

            Console.WriteLine("\n11. Удаляем знаменитость");
            repo.DelCelebrity(celebId);
            Console.WriteLine("Знаменитость удалена.");

            Console.WriteLine("\nКонец.");
        }
    }
}
