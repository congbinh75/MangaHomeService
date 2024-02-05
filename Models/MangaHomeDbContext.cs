using MangaHomeService.Models.Configurations;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService
{
    public class MangaHomeDbContext(DbContextOptions<MangaHomeDbContext> options, ITokenInfoProvider tokenInfoProvider) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Volume> Volumes { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<TitleRating> TitleRatings { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<OtherName> OtherNames { get; set; }
        public DbSet<ReadingList> ReadingLists { get; set; }
        public DbSet<CommentVote> CommentVotes { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<FeaturedTitle> FeaturedTitles { get; set; }
        public DbSet<ViewsCount> ViewsCounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MangaHome");
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ChapterConfiguration());
            modelBuilder.ApplyConfiguration(new CommentVoteConfiguration());
            modelBuilder.ApplyConfiguration(new FeaturedTitleConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new TitleConfiguration());
            modelBuilder.ApplyConfiguration(new TitleRatingConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUser = await Users.FirstOrDefaultAsync(u => u.Id == tokenInfoProvider.Id, cancellationToken: cancellationToken);

            var changedEntities = ChangeTracker.Entries();
            foreach (var changedEntity in changedEntities)
            {
                if (changedEntity.Entity is BaseEntity entity)
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
