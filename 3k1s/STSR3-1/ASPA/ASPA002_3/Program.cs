using Microsoft.AspNetCore.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // фильтр сообщений
        builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics", LogLevel.None);

        var app = builder.Build();

        // path к обработчику событий
        app.UseExceptionHandler("/error");

        app.MapGet("/", () => "Start");

        app.MapGet("/test1", () =>
        {
            throw new Exception("-- Exception Test --"); //пользовательское исключение
        });

        app.MapGet("/test2", () =>
        {
            int x = 0, y = 5, z = 0;
            z = y / x; // деление на 0 
            return "test2";
        });

        app.MapGet("/test3", () =>
        {
            int[] x = new int[3] { 1, 2, 3 };
            int y = x[3]; // выход за пределами массива
            return "test3";
        });

        app.Map("/error", async (ILogger<Program> logger, HttpContext context) =>
        {
            var exobj = context.Features.Get<IExceptionHandlerFeature>(); // добираемся к Exception
            await context.Response.WriteAsync("<h1>Oops!</h1>");
            logger.LogError(exobj?.Error, "ExceptionHandler");
        });

        app.Run();
    }
}
