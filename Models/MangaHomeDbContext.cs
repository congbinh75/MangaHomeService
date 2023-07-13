using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Models
{
    public class MangaHomeDbContext : DbContext
    {
        public MangaHomeDbContext(DbContextOptions<MangaHomeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MangaHome");
            base.OnModelCreating(modelBuilder);
        }
    }
}
