using DAL_Celebrity_Npgsql;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ASPA008_1
{
    public static class CelebritiesAPI
    {
        public static IServiceCollection AddCelebritiesConfiguration(this WebApplicationBuilder builder,
                                                                     string celebrityjson = "Celebrities.config.json")
        {
            builder.Configuration.AddJsonFile(celebrityjson);
            return builder.Services.Configure<CelebritiesConfig>(builder.Configuration.GetSection("Celebrities"));
        }

        public static IServiceCollection AddCelebritiesServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRepository, IRepository>((IServiceProvider p) =>
            {
                CelebritiesConfig config = p.GetRequiredService<IOptions<CelebritiesConfig>>().Value;
                return new Repository(config.ConnectionString);
            });

            builder.Services.AddSingleton<CelebrityTitles>((p) => new CelebrityTitles());

            builder.Services.AddSingleton<CountryCodes>((p) => new CountryCodes(p.GetRequiredService<IOptions<CelebritiesConfig>>().Value.ISO3166alpha2Path));

            return builder.Services;
        }

        public static IApplicationBuilder UseCelebritiesErrorHandler(this IApplicationBuilder app, string prefix)
        {
            return app.UseMiddleware<CelebritiesErrorHandler>(prefix);
        }

        public class CelebrityTitles
        {
            public string Head { get; } = "Celebrities Dictionary Internet Service";
            public string Title { get; } = "Celebrities";
            public string Copyright { get; } = @"Copyright BSTU";
        }

        public class CountryCodes: List<CountryCodes.ISOCountryCodes>
        {
            public record ISOCountryCodes(string code, string countryLabel);
            public CountryCodes(string jscountrycodespath): base()
            {
                if (File.Exists(jscountrycodespath))
                {
                    FileStream fs = new FileStream(jscountrycodespath, FileMode.OpenOrCreate, FileAccess.Read);
                    List<ISOCountryCodes>? cc = JsonSerializer.DeserializeAsync<List<ISOCountryCodes>?>(fs).Result;
                    if (cc != null) this.AddRange(cc);
                }
            }
        }


        public static RouteHandlerBuilder MapCelebrities(this IEndpointRouteBuilder routeBuilder, string prefix = "/api/Celebrities")
        {
            var celebrities = routeBuilder.MapGroup(prefix);
            celebrities.MapGet("/", (IRepository repo) => repo.GetAllCelebrities());
            
            celebrities.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                Celebrity? celebrity = repo.GetCelebrityById(id);
                if (celebrity == null) throw new FoundByIdException($"/Celebrities, Celebrity Id = {id}");
                return Results.Json(celebrity);
            });

            celebrities.MapGet("/Lifeevents/{id:int:min(1)}", (IRepository repo, int id) => repo.GetCelebrityByLifeeventId(id));
            celebrities.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) => {
                var c = repo.GetCelebrityById(id);
                if (c != null)
                {
                    repo.DelCelebrity(id);
                    return Results.Json(c);
                }
                throw new DelByIdException($"DELETE /Celebrities error, Id = {id}");
            });
            celebrities.MapPost("/", (IRepository repo, Celebrity celebrity) => {
                if (repo.AddCelebrity(celebrity))
                {
                    return Results.Json(celebrity);
                }
                throw new AddCelebrityException("/Celebrities error, id == null");
            });
            celebrities.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) => {
                var c = repo.GetCelebrityById(id);
                if (c != null)
                {
                    if (repo.UpdCelebrity(id, celebrity) != null)
                    {
                        return Results.Json(repo.GetCelebrityById(id));
                    }
                    throw new UpdException($"/Celebrities error, Id = {id}");
                }
                throw new FoundByIdException($"/Celebrities, Celebrity Id = {id}");
            });

            return celebrities.MapGet("/photo/{fname}", async (IOptions<CelebritiesConfig> iconfig, HttpContext context, string fname) =>
            {
                    var path = Path.Combine(iconfig.Value.PhotosFolder, fname);

                    if (!File.Exists(path))
                    {
                        return Results.NotFound();
                    }
                    return Results.File(path, "image/jpeg");
            });
        }

        public static RouteHandlerBuilder MapPhotoCelebrities(this IEndpointRouteBuilder routeBuilder, string prefix = "/Photos")
        {
            if(string.IsNullOrEmpty(prefix)) prefix = routeBuilder.ServiceProvider.GetRequiredService<IOptions<CelebritiesConfig>>().Value.PhotosRequestPath;
            return routeBuilder.MapGet($"{prefix}/{{fname}}", async (IOptions<CelebritiesConfig> iconfig, HttpContext context, string fname) =>
            {
                CelebritiesConfig config = iconfig.Value;
                string filepath = Path.Combine(config.PhotosFolder, fname);
                FileStream file = File.OpenRead(filepath);
                BinaryReader br = new BinaryReader(file);
                BinaryWriter bw = new BinaryWriter(context.Response.BodyWriter.AsStream());
                int n = 0;
                byte[] buffer = new byte[2048];
                context.Response.ContentType = "image/jpeg";
                context.Response.StatusCode = StatusCodes.Status200OK;
                while((n = await br.BaseStream.ReadAsync(buffer, 0, 2048)) > 0) await bw.BaseStream.WriteAsync(buffer, 0, n);
                br.Close();
                bw.Close();
            });
        }


        public static RouteHandlerBuilder MapLifeevents(this IEndpointRouteBuilder routeBuilder, string prefix = "/api/Lifeevents")
        {
            var lifeevents = routeBuilder.MapGroup(prefix);
            lifeevents.MapGet("/", (IRepository repo) => repo.GetAllLifeevents());
            lifeevents.MapGet("/{id:int:min(1)}", (IRepository repo, int id) => repo.GetLifeeventById(id));
            lifeevents.MapGet("/Celebrities/{id:int:min(1)}", (IRepository repo, int id) => repo.GetLifeeventsByCelebrityId(id));
            lifeevents.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) => {
                var l = repo.GetLifeeventById(id);
                if (l != null)
                {
                    repo.DelLifeevent(id);
                    return Results.Json(l);
                }
                throw new FileNotFoundException($"404004:Lifeevent Id = {id}");
            });
            lifeevents.MapPost("/", (IRepository repo, Lifeevent lifeevent) => {
                if (repo.AddLifeevent(lifeevent))
                {
                    return Results.Json(lifeevent);
                }
                throw new Exception($"ERROR: AddLifeevent");
            });
            return lifeevents.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Lifeevent lifeevent) => {
                var l = repo.GetLifeeventById(id);
                if (l != null)
                {
                    if (repo.UpdLifeevent(id, lifeevent))
                    {
                        return Results.Json(repo.GetLifeeventById(id));
                    }
                    throw new Exception($"ERROR: UpdLifeevent");
                }
                throw new FileNotFoundException($"404004:Lifeevent Id = {id}");
            });
        }
    }


    public class FoundByIdException : Exception
    {
        public FoundByIdException(string message) : base($"Found by Id: {message}") { }
    };

    public class SaveException : Exception
    {
        public SaveException(string message) : base($"SaveChanges error: {message}") { }
    };

    public class AddCelebrityException : Exception
    {
        public AddCelebrityException(string message) : base($"AddCelebrityException error: {message}") { }
    };

    public class DelByIdException : Exception
    {
        public DelByIdException(string message) : base($"Delete by Id: {message}") { }
    };

    public class UpdException : Exception
    {
        public UpdException(string message) : base($"UpdException error: {message}") { }
    };

}
