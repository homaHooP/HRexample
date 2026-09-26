using Microsoft.AspNetCore.Mvc;
using HRexample.Models;
using HRexample.Services;
using HRexample.Data;
using Microsoft.EntityFrameworkCore;

namespace HrRequirementsApp.Controllers
{
    public class JobRequirementsController (bucketService bs, AppDbContext db) : Controller
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
        public async Task<IActionResult> Create(JobRequirement jobRequirement)
        {
            if (!ModelState.IsValid)
            {
                return View(jobRequirement);
            }

            await db.JobRequirements.AddAsync(jobRequirement);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
