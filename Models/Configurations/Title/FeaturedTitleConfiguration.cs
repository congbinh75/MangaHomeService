using MangaHomeService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaHomeService.Models.Configurations
{
    public class FeaturedTitleConfiguration : IEntityTypeConfiguration<FeaturedTitle>
    {
        public void Configure(EntityTypeBuilder<FeaturedTitle> builder)
        {
            builder.HasNoKey();
        }
    }
}