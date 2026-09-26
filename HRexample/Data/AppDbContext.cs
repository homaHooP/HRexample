using Microsoft.EntityFrameworkCore;
using HRexample.Models;

namespace HRexample.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<JobRequirement> JobRequirements { get; set; } = null!;
        public DbSet<Application> Applications { get; set; } = null!;
    }
}
