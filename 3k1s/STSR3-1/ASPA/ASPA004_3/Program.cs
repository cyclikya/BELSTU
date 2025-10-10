using DAL004;
using Microsoft.AspNetCore.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        Repository.JSONFileName = "Celebrities.json";
        using (IRepository repository = new Repository("Celebrities"))
        {
            app.UseExceptionHandler("/Celebrities/Error");

            app.MapGet("/Celebrities", () => repository.getAllCelebrities());
            app.MapGet("/Celebrities/{id:int}", (int id) =>
            {
                Celebrity? celebrity = repository.getCelebrityById(id);
                if (celebrity == null)
                {
                    throw new FoundByIdException($"Celebrity Id = {id}");
                }
                return celebrity;
            });
            app.MapPost("/Celebrities", (Celebrity celebrity) =>
            {
                int? id = repository.addCelebrity(celebrity);
                if (id == null) throw new AddCelebrityException("/Celebrities error, id == null");
                if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
            });
            app.MapDelete("/Celebrities/{id:int}", (int id) =>
            {
                if (!repository.delCelebrityById(id))
                {
                    throw new DeleteCelebrityException($"Delete by Id:DELETE /Celebrities error, Id = {id}");
                }
                if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                return $"Celebrity with Id = {id} deleted";
            });
            // Добавляем
            app.MapPut("/Celebrities/{id:int}", (int id, Celebrity celebrity) =>
            {
                int? Id = repository.updCelebrityById(id, celebrity);
                if (Id == null) throw new UpdateCelebrityException("Put /Celebrities error, id == null");
                if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                return new Celebrity((int)Id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
            });

            app.MapFallback((HttpContext ctx) =>
            {
                return Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" });
            });
            app.Map("/Celebrities/Error", (HttpContext ctx) =>
            {
                Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                IResult rc = Results.Problem(detail: "Panic", instance: app.Environment.EnvironmentName, title: "ASPA004", statusCode: 500);
                if (ex != null)
                {
                    if (ex is UpdateCelebrityException) rc = Results.NotFound(ex.Message);
                    if (ex is DeleteCelebrityException) rc = Results.NotFound(ex.Message);
                    if (ex is FileNotFoundException) rc = Results.Problem(title: "ASPA00", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is FoundByIdException) rc = Results.NotFound(ex.Message);
                    if (ex is BadHttpRequestException) rc = Results.BadRequest(ex.Message);
                    if (ex is SaveException) rc = Results.Problem(title: "ASPA004/SaveChanges", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is AddCelebrityException) rc = Results.Problem(title: "ASPA004/addCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                }
                return rc;
            });
        }
        app.Run();
    }
}