using ASPA008_1.Models;
using DAL_Celebrity_Npgsql;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ASPA008_1.Controllers
{
    public class CelebritiesController : Controller
    {
        IRepository repo;
        IOptions<CelebritiesConfig> config;

        public Celebrity? Celebrity {  get; set; }
        public CelebritiesController(IRepository repo, IOptions<CelebritiesConfig> config)
        {
            this.repo = repo;
            this.config = config;
        }

        public record IndexModel(string PhotosRequestPath, List<Celebrity> Celebrities);

        public record HumanModel(string photosrequespath, Celebrity celebrity, List<Lifeevent> lifeevents, Dictionary<string, string>? references);

        public IActionResult Index()
        {
            return View(new IndexModel(config.Value.PhotosRequestPath, repo.GetAllCelebrities()));
        }
        [InfoAsyncActionFilter(infotype:"Wikipedia, Facebook")]
        public IActionResult Human(int id)
        {
            IActionResult rc = NotFound();
            Celebrity? celebrity = repo.GetCelebrityById(id);
            ViewData["Celebrity"] = celebrity;
            Dictionary<string, string>? references = (Dictionary<string, string>?)HttpContext.Items[InfoAsyncActionFilter.Wikipedia];

            if (celebrity != null) {
                rc = View(new HumanModel(config.Value.PhotosRequestPath, (Celebrity)celebrity, repo.GetLifeeventsByCelebrityId(id), references));
            }
            return rc;
        }

        public IActionResult EditHumanForm(int id)
        {
            var celebrity = repo.GetCelebrityById(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            ViewData["Celebrity"] = celebrity;
            return View("EditHumanForm", config.Value.PhotosRequestPath);
        }

        public IActionResult DeleteHumanForm(int id)
        {
            var celebrity = repo.GetCelebrityById(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            ViewData["Celebrity"] = celebrity;
            return View("DeleteHumanForm", config.Value.PhotosRequestPath);
        }

        public IActionResult NewHumanForm()
        {
            return View("NewHumanForm", config.Value.PhotosRequestPath);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCelebrity(string fullname, string Nationality, IFormFile upload)
        {
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(Nationality) || upload == null || upload.Length == 0)
            {
                ModelState.AddModelError("", "All fields are required.");
                return View("NewHumanForm", config.Value.PhotosRequestPath);
            }

            string fileName = Path.GetFileName(upload.FileName);

            ViewData["Confirm"] = true;
            ViewData["Celebrity"] = new Celebrity
            {
                FullName = fullname,
                Nationality = Nationality,
                ReqPhotoPath = upload.FileName
            };

            return View("NewHumanForm", config.Value.PhotosRequestPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCelebrity(int id, string fullname, string Nationality, IFormFile upload, string _method)
        {
            if (_method == "put")
            {
                var celebrity = repo.GetCelebrityById(id);
                if (celebrity != null)
                {
                    celebrity.FullName = fullname;
                    celebrity.Nationality = Nationality;
                    if (upload != null && upload.Length > 0)
                    {
                        string fileName = Path.GetFileName(upload.FileName);
                        string filePath = Path.Combine(config.Value.PhotosFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            upload.CopyTo(stream);
                        }
                        celebrity.ReqPhotoPath = fileName;
                    }
                    repo.UpdCelebrity(celebrity.Id, celebrity);
                }

                return RedirectToAction("Index"); 
            }
            return RedirectToAction("EditHumanForm", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmAddCelebrity(string fullname, string Nationality, string uploadFileName)
        {
            repo.AddCelebrity(new Celebrity
            {
                FullName = fullname,
                Nationality = Nationality,
                ReqPhotoPath = uploadFileName
            });

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCelebrity(int id, string _method)
        {
            if (_method == "delete")
            {
                repo.DelCelebrity(id);

                return RedirectToAction("Index");
            }
            return RedirectToAction("DeleteHumanForm", new { id = id });
        }
    }
}
