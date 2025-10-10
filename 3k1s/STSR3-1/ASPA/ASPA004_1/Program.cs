using Microsoft.AspNetCore.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DAL004;
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
        if (celebrity == null) throw new FoundByIdException($"Celebrity Id = {id}");
        return celebrity;
    });
    app.MapPost("/Celebrities", (Celebrity celebrity) =>
    {
        int? id = repository.addCelebrity(celebrity);
        if (id == null) throw new AddCelebrityException("/Celebrities error, id == null");
        if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0 ");
        return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);

    });

    app.MapFallback((HttpContext ctx) => Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));
    app.Map("/Celebrities/Error", (HttpContext ctx) => {
        Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        IResult rc = Results.Problem(detail: $"Panic", instance: app.Environment.EnvironmentName, title: "ASPA004", statusCode: 500);
        //IResult rc = Results.Problem(detail: "Could not find file", instance: app.Environment.EnvironmentName, title: "ASPA004", statusCode: 500);
        if (ex != null)
        {

            if (ex is FoundByIdException) rc = Results.NotFound(ex.Message);
            if (ex is BadHttpRequestException) rc = Results.BadRequest(ex.Message);
            if (ex is SaveException)
                rc = Results.Problem(title: "ASPA064/SaveChanges", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 560);
            if (ex is AddCelebrityException)
                rc = Results.Problem(title: "ASPA084/addCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
        }
        return rc;
    });
    app.Run();
}