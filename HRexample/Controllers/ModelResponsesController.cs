using Microsoft.AspNetCore.Mvc;
using HRexample.Data;
using HRexample.Models;
using Microsoft.EntityFrameworkCore;
using HRexample.Services;

namespace HRexample.Controllers
{
    public class ModelResponsesController(AppDbContext _db, GeminiService _ai, bucketService _bs, ILogger<ModelResponsesController> _logger) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var applications = await _db.Applications
                .Include(a => a.JobRequirement)
                .Include(a => a.ModelResponse)
                .OrderBy(a => a.JobRequirement!.Position)
                .ThenByDescending(a => a.ModelResponse != null ? a.ModelResponse.PercentageFitForJob : -1)
                .ThenBy(a => a.CreatedAt)
                .Where(a => a.ModelResponse != null)
                .ToListAsync();

            // Метрики: кількість поданих заявок по датах та середня відповідність
            var perDay = await _db.Applications
                .GroupBy(a => a.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            ViewBag.ChartLabels = perDay.Select(x => x.Date.ToString("dd.MM.yyyy")).ToList();
            ViewBag.ChartValues = perDay.Select(x => x.Count).ToList();
            ViewBag.AverageFit = applications.Any() ? Math.Round(applications.Average(a => a.ModelResponse!.PercentageFitForJob)) : (double?)null;

            return View(applications);
        }

        [HttpGet("modelresponse/{applicationId}")]
        public IActionResult Details([FromRoute] Guid applicationId)
        {
            var application = _db.Applications.Include(x=>x.JobRequirement).Include(a=>a.ModelResponse).FirstOrDefault(a => a.Id == applicationId);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }
        [HttpPost("modelresponse/{applicationId}/analyze")]
        public async Task<IActionResult> Analyze([FromRoute] Guid applicationId)
        {
            var application = _db.Applications.Include(x=>x.JobRequirement).Include(x => x.ModelResponse).FirstOrDefault(a => a.Id == applicationId);
            if (application == null)
            {
                return NotFound();
            }
            try
            {
                string resumeText = await _bs.ReadPdfTextAsync(application.s3FileKey);
                string reqirementsText = await _bs.ReadTextAsync(application.JobRequirement.s3FileKey);
                var modelResponse = await _ai.AnalyzeResume(reqirementsText,resumeText);

                var Old = application.ModelResponse;
                _db.ModelResponses.Add(modelResponse);
                application.ModelResponse = modelResponse;
                if (Old != null)
                {
                    _db.ModelResponses.Remove(Old);
                }
                await _db.SaveChangesAsync();
                TempData["Success"] = "Огляд ШІ успішно оновлено.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI analysis failed for application {ApplicationId}", applicationId);
                TempData["Error"] = "Не вдалося отримати огляд ШІ. Спробуйте ще раз пізніше.";
            }
            return RedirectToAction("Details", new { applicationId = application.Id });
        }
    }
}
