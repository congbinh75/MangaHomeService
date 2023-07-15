using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MangaHomeService.Models
{
    public class MangaHomeDbContext : DbContext
    {
        public MangaHomeDbContext(DbContextOptions<MangaHomeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MangaHome");
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            string currentUser = "";
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                currentUser = claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "";
            }

            var changedEntities = ChangeTracker.Entries();
            foreach (var changedEntity in changedEntities)
            {
                if (changedEntity.Entity is BaseModel entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedTime = now;
                            entity.UpdatedTime = now;
                            entity.CreatedBy = currentUser;
                            entity.UpdatedBy = currentUser;
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(entity).Property(x => x.CreatedTime).IsModified = false;
                            entity.UpdatedTime = now;
                            entity.UpdatedBy = currentUser;
                            break;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
