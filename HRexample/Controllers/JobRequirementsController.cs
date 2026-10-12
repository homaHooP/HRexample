using HRexample.Data;
using HRexample.Models;
using HRexample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace HrRequirementsApp.Controllers
{
    public class JobRequirementsController(bucketService bs, AppDbContext db) : Controller
    {
        public async Task<ActionResult> Index()
        {
            List<JobRequirement> requirements = await db.JobRequirements.ToListAsync();
            return View(requirements);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobRequirement jobRequirement, IFormFile? requirementFile)
        {
            if (requirementFile == null || requirementFile.Length == 0)
            {
                ModelState.AddModelError("requirementFile", "Будь ласка, прикріпіть файл вимог.");
            }
            else
            {
                var extension = Path.GetExtension(requirementFile.FileName).ToLowerInvariant();
                if (extension != ".txt" || requirementFile.ContentType != "text/plain")
                {
                    ModelState.AddModelError("requirementFile", "Дозволено завантажувати лише файли у форматі TXT.");
                }

                const long maxSizeBytes = 5 * 1024 * 1024;
                if (requirementFile.Length > maxSizeBytes)
                {
                    ModelState.AddModelError("requirementFile", "Розмір файлу не повинен перевищувати 5 МБ.");
                }
            }
            if (!ModelState.IsValid)
            {
                return View(jobRequirement);
            }
            var key = $"req/{jobRequirement.Id}.txt";

            await using var stream = requirementFile!.OpenReadStream();
            await bs.UploadObjectAsync(key, stream, "text/plain");
            jobRequirement.s3FileKey = key;
            await db.JobRequirements.AddAsync(jobRequirement);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
