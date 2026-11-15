using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services
    .AddOcelot(builder.Configuration)
    .AddCustomLoadBalancer<WeightedLoadBalancer>((route, provider) =>
        new WeightedLoadBalancer(route.DownstreamAddresses
            .Select(a => new ServiceHostAndPort(a.Host, a.Port))
            .ToList()));

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

await app.UseOcelot();

app.Run();
