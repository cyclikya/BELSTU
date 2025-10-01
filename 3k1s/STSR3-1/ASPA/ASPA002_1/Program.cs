var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWelcomePage("/aspnetcore");

app.MapGet("/aspnetcore", () => "");

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
