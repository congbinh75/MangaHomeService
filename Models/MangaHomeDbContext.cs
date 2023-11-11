using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MangaHomeService.Models
{
    public class MangaHomeDbContext : DbContext
    {
        public MangaHomeDbContext(DbContextOptions<MangaHomeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Volume> Volumes { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Demographic> Demographics { get; set; }
        public DbSet<ChapterTracking> ChapterTrackings { get; set; }
        public DbSet<TitleRating> TitleRatings { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<TitleRequest> TitleRequests { get; set; }
        public DbSet<ChapterRequest> ChapterRequests { get; set; }
        public DbSet<GroupRequest> GroupRequests { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<TitleOtherName> TitleOtherNames { get; set; }

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
