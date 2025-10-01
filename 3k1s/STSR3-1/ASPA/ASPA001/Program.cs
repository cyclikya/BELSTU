internal class Program
{
    private static void Main(string[] args)
    {
        // Создаём билдер веб-приложения, используя аргументы командной строки
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем в билдер сервис для логирования HTTP-запросов и ответов
        builder.Services.AddHttpLogging(options =>
        {
            // Указываем, что нужно логировать все поля HTTP-запроса и ответа
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        });

        // Строим приложение на основе конфигурации билдера
        var app = builder.Build();

        // Включаем middleware для логирования HTTP-запросов и ответов
        app.UseHttpLogging();

        // Создаём GET-эндпоинт на корневом маршруте "/", который возвращает простую строку
        app.MapGet("/", () => "Моё первое ASPA");

        // Запускаем веб-приложение, чтобы слушать входящие HTTP-запросы
        app.Run();
    }
}
