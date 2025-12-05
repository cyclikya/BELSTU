using DAL_Celebrity_Npgsql;
using Lab7.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IO;

namespace Lab7.Pages
{
    public class NewCelebrityModel : PageModel
    {
        public IRepository repository;
        public string PhotosRequestPath { get; set; }
        public string PhotosFolder { get; set; }
        public Celebrity? Celebrity { get; set; }

        public NewCelebrityModel(IRepository repository, IOptions<CelebritiesConfig> config) {
            this.repository = repository;
            PhotosRequestPath = config.Value.PhotosRequestPath;
            PhotosFolder = config.Value.PhotosFolder;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnGetConfirm(string fullname, string nationality, string filename)
        {
            ViewData["Confirm"] = true;
            Celebrity = new Celebrity()
            {
                FullName = fullname,
                Nationality = nationality,
                ReqPhotoPath = filename,
            };
            return Page();
        }
        public IActionResult OnPost(
            [FromForm]string? fullname,
            [FromForm]string? nationality,
            IFormFile upload,
            string? press,
            string? filename)
        {
            IActionResult rc = RedirectToPage("Celebrities");
            if(string.IsNullOrEmpty(press))
            {
                if (upload == null || upload.Length == 0)
                {
                    return RedirectToPage("NewCelebrity");
                }

                string fn = Path.GetFileName(Path.GetTempFileName());
                string fp = Path.Combine(PhotosFolder, fn);
                
                try
                {
                    using (FileStream file = new FileStream(fp, FileMode.CreateNew))
                    {
                        upload.CopyTo(file);
                    }
                    rc = RedirectToPage("NewCelebrity", "Confirm", new { filename = fn, fullname, nationality });
                }
                catch (IOException)
                {
                    // Файл уже существует, попробуем с другим именем
                    fn = Path.GetFileName(Path.GetTempFileName());
                    fp = Path.Combine(PhotosFolder, fn);
                    using (FileStream file = new FileStream(fp, FileMode.Create))
                    {
                        upload.CopyTo(file);
                    }
                    rc = RedirectToPage("NewCelebrity", "Confirm", new { filename = fn, fullname, nationality });
                }
            }
            else if(press.Equals("Confirm"))
            {
                if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(nationality) || string.IsNullOrEmpty(filename))
                {
                    return RedirectToPage("NewCelebrity");
                }

                string sourcePath = Path.Combine(PhotosFolder, filename);
                if (!System.IO.File.Exists(sourcePath))
                {
                    return RedirectToPage("NewCelebrity");
                }

                string newFileName = $"{fullname.Replace(" ", "_")}.{Path.GetFileNameWithoutExtension(filename)}.jpg";
                string destPath = Path.Combine(PhotosFolder, newFileName);
                
                try
                {
                    System.IO.File.Move(sourcePath, destPath);
                    repository.AddCelebrity(new Celebrity
                    {
                        FullName = fullname,
                        Nationality = nationality,
                        ReqPhotoPath = newFileName,
                    });
                    rc = RedirectToPage("Celebrities");
                }
                catch (IOException)
                {
                    // Ошибка при перемещении файла
                    return RedirectToPage("NewCelebrity");
                }
            }
            else
            {
                rc = RedirectToPage("NewCelebrity");
            }
            return rc;
        }
    }
}
