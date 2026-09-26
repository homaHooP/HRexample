using HRexample.Data;
using HRexample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRexample.Models;

namespace HRexample.Controllers
{
    public class ApplicationsController(AppDbContext _context, bucketService _bs) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var applications = await _context.Applications.ToListAsync();
            return View(applications);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application, IFormFile? resumeFile)
        {
            if (resumeFile == null || resumeFile.Length == 0)
            {
                ModelState.AddModelError("resumeFile", "Будь ласка, прикріпіть файл резюме.");
            }
            else
            {
                var extension = Path.GetExtension(resumeFile.FileName).ToLowerInvariant();
                if (extension != ".pdf" || resumeFile.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("resumeFile", "Дозволено завантажувати лише файли у форматі PDF.");
                }

                const long maxSizeBytes = 5 * 1024 * 1024; 
                if (resumeFile.Length > maxSizeBytes)
                {
                    ModelState.AddModelError("resumeFile", "Розмір файлу не повинен перевищувати 5 МБ.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(application);
            }

            await using var stream = resumeFile!.OpenReadStream();
            await _bs.UploadObjectAsync(application.s3FileKey, stream, "application/pdf");

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> GetResumeLink(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            var url = await _bs.GenerateLinkAsync(application.s3FileKey);
            return Json(new { url });
        }
    }
}
