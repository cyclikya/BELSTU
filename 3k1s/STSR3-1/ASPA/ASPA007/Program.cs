using Lab7;
using Lab7.Configuration;
using Lab7.Middlewares;
using Microsoft.Extensions.FileProviders;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.AddCelebritiesConfiguration();
builder.AddCelebritiesServices();

builder.Services.AddRazorPages();
builder.Services.AddRazorPages(o =>
{
    o.Conventions.AddPageRoute("/Celebrities", "/");
    o.Conventions.AddPageRoute("/NewCelebrity", "/0");
    o.Conventions.AddPageRoute("/Celebrity", "/Celebrities/{id:int:min(1)}");
    o.Conventions.AddPageRoute("/Celebrity", "/{id:int:min(1)}");

});

var app = builder.Build();


var celebritiesOptions = builder.Configuration
    .GetSection("Celebrities")
    .Get<CelebritiesConfig>();

try
{
    var masterConnectionString = celebritiesOptions.ConnectionString.Replace("Database=Celebrity", "Database=postgres");
    using (var masterConnection = new NpgsqlConnection(masterConnectionString))
    {
        masterConnection.Open();
        using (var command = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = 'Celebrity'", masterConnection))
        {
            var exists = command.ExecuteScalar() != null;
            if (!exists)
            {
                using (var createCommand = new NpgsqlCommand(
                    "CREATE DATABASE \"Celebrity\"", masterConnection))
                {
                    createCommand.ExecuteNonQuery();
                }
            }
        }
    }
    
    var context = new DAL_Celebrity_Npgsql.Context(celebritiesOptions.ConnectionString);
    context.Database.EnsureCreated();
}
catch (Exception ex)
{
    // Если автоматическое создание не удалось, используем Init
    // Init использует статическое поле, поэтому нужно сначала создать экземпляр
    try
    {
        var init = new DAL_Celebrity_Npgsql.Init(celebritiesOptions.ConnectionString);
        DAL_Celebrity_Npgsql.Init.Execute(delete: false, create: true);
    }
    catch
    {
        // Игнорируем ошибку, возможно БД уже существует или создана вручную
        // В этом случае пользователю нужно создать БД вручную
    }
}
app.UseStaticFiles();

// Photos существует
if (!Directory.Exists(celebritiesOptions.PhotosFolder))
{
    Directory.CreateDirectory(celebritiesOptions.PhotosFolder);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        celebritiesOptions.PhotosFolder),
    RequestPath = "/Photos"
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

app.MapCelebrities();
app.MapLifeevents();
app.MapPhotoCelebrities();

app.Run();
