using DAL003;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var picturesPath = Path.Combine(Directory.GetCurrentDirectory(), "Photo");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(picturesPath),
    RequestPath = "/Photo/download",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Add("Content-Disposition", "attachment");
    }
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Photo")),
    RequestPath = "/Photo"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(picturesPath),
    RequestPath = "/Photo/download"
});

Repository.JSONFileName = "Celebrities.json";
using (IRepository repository = new Repository("Celebrities"))
{
    app.MapGet("/Celebrities", () => repository.getAllCelebrities());
    app.MapGet("/Celebrities/{id:int}", (int id) => repository.getCelebrityById(id));
    app.MapGet("/Celebrities/BySurname/{surname}", (string surname) => repository.getCelebritiesBySurname(surname));
    app.MapGet("/Celebrities/PhotoPathById/{id:int}", (int id) => repository.getPhotoPathId(id));
    app.MapGet("/", () => "Hello World!");

    app.Run();
}


//localhost:7278/Celebrities
//localhost:7278/Photo/download
//localhost:7278/Celebrities/1
//localhost:7278/Celebrities/BySurname/Chomsky
//localhost:7278/Celebrities/PhotoPathById/6
//localhost:7278/Celebrities/Photo/Dijkstra.jpg