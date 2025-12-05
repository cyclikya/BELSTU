using DAL_Celebrity_Npgsql;
using Lab7.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Lab7.Pages
{
    public class СelebritiesModel(IRepository Repository, IOptions<CelebritiesConfig> config) : PageModel
    {
        public List<Celebrity> Celebrities { get; set; } = new List<Celebrity>();

        public string PhotosRequestPath { get; set; } = config.Value.PhotosRequestPath;
        public IActionResult OnGet()
        {
            Celebrities = Repository.GetAllCelebrities().ToList();
            return Page();
        }
    }
}
