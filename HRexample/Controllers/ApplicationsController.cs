using HRexample.Data;
using HRexample.Models;
using HRexample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HRexample.Controllers
{
    public class ApplicationsController(AppDbContext _db, bucketService _bs) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var applications = await _db.Applications.Include(x=>x.JobRequirement).Include(x=>x.ModelResponse).OrderBy(a=>a.JobRequirement.Position).ThenBy(x=>x.CreatedAt).ToListAsync();
            return View(applications);
        }

        [HttpGet("apply/{positionId}")]
        public async Task<IActionResult> Create([FromRoute] Guid positionId)
        {
            var jobRequirement = await _db.JobRequirements.FindAsync(positionId);
            if (jobRequirement == null)
            {
                return NotFound();
            }
            var application = new Application
            {
                JobRequirementId = jobRequirement.Id
            };
            return View(application);
        }

        [HttpPost("apply/{positionId}")]
        public async Task<IActionResult> Create([FromRoute] Guid positionId, Application application, IFormFile? resumeFile)
        {
            var jobRequirement = await _db.JobRequirements.FindAsync(positionId);
            if (jobRequirement == null)
            {
                return NotFound();
            }
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
            var key = $"app/{application.Id}.pdf";

            await using var stream = resumeFile!.OpenReadStream();
            await _bs.UploadObjectAsync(key, stream, "application/pdf");

            application.s3FileKey = key;
            application.JobRequirementId = jobRequirement.Id;
            await _db.Applications.AddAsync(application);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var application = await _db.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> GetResumeLink(Guid id)
        {
            var application = await _db.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            var url = await _bs.GenerateLinkAsync(application.s3FileKey);
            return Json(new { url });
        }
    }
}
