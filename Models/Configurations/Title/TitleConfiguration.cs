using MangaHomeService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaHomeService.Models.Configurations
{
    public class TitleConfiguration : IEntityTypeConfiguration<Title>
    {
        public void Configure(EntityTypeBuilder<Title> builder)
        {
            builder.HasMany(t => t.Authors)
                .WithMany(p => p.AuthoredTitles)
                .UsingEntity(j => j.ToTable("TitlesAuthors"));

            builder.HasMany(t => t.Artists)
                .WithMany(p => p.IllustratedTitles)
                .UsingEntity(j => j.ToTable("TitlesArtists"));

            builder.HasMany(t => t.Gernes)
                .WithMany(g => g.Titles)
                .UsingEntity(j => j.ToTable("GenresOfTitles"));

            builder.HasMany(t => t.Themes)
                .WithMany(t => t.Titles)
                .UsingEntity(j => j.ToTable("ThemesOfTitles"));

            builder.HasMany(t => t.Demographics)
                .WithMany(d => d.Titles)
                .UsingEntity(j => j.ToTable("DemogrphicsOfTitles"));
        }
    }
}