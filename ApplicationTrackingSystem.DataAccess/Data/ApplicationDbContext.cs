using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTrackingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<JobPost>JobPosts { get; set; }
        public DbSet<Applyjob>applyjobs { get; set; }
        public DbSet<FormLinks> formLinks { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
