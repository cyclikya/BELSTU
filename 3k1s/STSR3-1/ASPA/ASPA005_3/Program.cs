using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();


app.UseExceptionHandler("/Error");

// ---A-----------
app.MapGet("/A/{x:int:min(-3):max(100)}", (HttpContext context, [FromRoute] int x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});

app.MapPost("/A/{x:int:min(0):max(100)}", (HttpContext context, [FromRoute] int x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});

app.MapPut("/A/{x:int:min(1):max(100)}/{y:int:min(1):max(100)}", (HttpContext context, [FromRoute] int x, [FromRoute] int y) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y });
});

app.MapDelete("/A/{x:int:min(1):max(100)}/{y:int:min(0):max(100)}", (HttpContext context, [FromRoute] int x, [FromRoute] int y) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y });
});

// ---B-----------
app.MapGet("/B/{x:float}", (HttpContext context, [FromRoute] float x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});

app.MapPost("/B/{x:float}/{y:float}", (HttpContext context, [FromRoute] float x, [FromRoute] float y) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y });
});

app.MapDelete("/B/{x:float}-{y:float}", (HttpContext context, [FromRoute] float x, [FromRoute] float y) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y });
});

// ---C-----------
app.MapGet("/C/{x:bool}", (HttpContext context, [FromRoute] bool x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});

app.MapPost("/C/{x:bool},{y:bool}", (HttpContext context, [FromRoute] bool x, [FromRoute] bool y) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y });
});

// ---D-----------
app.MapGet("/D/{x:datetime}", (HttpContext context, [FromRoute] DateTime x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});

app.MapPost("/D/{x:datetime}|{y:datetime}", (HttpContext context, [FromRoute] DateTime x, [FromRoute] DateTime y) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y });
});

// ---E-----------
app.MapGet("/E/{x:minlength(2):maxlength(12)}", (HttpContext context, [FromRoute] string x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});

app.MapPut("/E/{x:minlength(2):maxlength(12)}", (HttpContext context, [FromRoute] string x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x });
});



// ---F-----------
//email
app.MapPut("/F/{x:regex(^[\\w.]+@[\\w.]+\\.by$)}", (HttpContext context, [FromRoute] string x) =>
{
    return Results.Ok(new { path = context.Request.Path.Value, x, y = (string?)null });
});

app.MapFallback((HttpContext ctx) =>
{
    return Results.NotFound(new { message = $"path {ctx.Request.Path.Value} not supported" });
});

app.Map("/Error", (HttpContext ctx) =>
{
    Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
    return Results.Ok(new { message = ex?.Message });
});

app.Run();