using MangaHomeService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaHomeService.Models.Configurations
{
    public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
    {
        public void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.HasQueryFilter(e => !e.IsRemoved);

            builder.HasMany(c => c.Pages)
                .WithOne(c => c.Chapter)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}