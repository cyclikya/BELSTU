using DAL_Celebrity_Npgsql;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPA008_1
{
    public class InfoAsyncActionFilter: Attribute, IAsyncActionFilter
    {
        public static readonly string Wikipedia = "WIKI";
        public static readonly string Facebook = "FACE";

        string infotype;
        public InfoAsyncActionFilter(string infotype = "") { 
            this.infotype = infotype.ToUpper();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IRepository? repo = context.HttpContext.RequestServices.GetService<IRepository>();
            int id = (int)(context.ActionArguments["id"] ?? -1);
            Celebrity? celebrity = repo?.GetCelebrityById(id);
            
            if (celebrity != null)
            {
                // Проверяем, нужно ли получать данные из Wikipedia
                if (infotype.Contains(Wikipedia) || infotype.Contains("WIKIPEDIA"))
                {
                    var wikiRefs = await WikiInfoCelebrity.GetRefereces(celebrity.FullName);
                    context.HttpContext.Items[Wikipedia] = wikiRefs;
                    System.Diagnostics.Debug.WriteLine($"Wikipedia references loaded: {wikiRefs.Count} for {celebrity.FullName}");
                }
                if (infotype.Contains(Facebook) || infotype.Contains("FACEBOOK"))
                {
                    context.HttpContext.Items[Facebook] = getFromFace(celebrity.FullName);
                }
            }
            
            await next();
        }
        string getFromFace(string fullname)
        {
            return "Info from Face";
        }
    }
}
