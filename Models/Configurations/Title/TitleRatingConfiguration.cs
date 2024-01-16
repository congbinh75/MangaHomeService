using MangaHomeService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaHomeService.Models.Configurations
{
    public class TitleRatingConfiguration : IEntityTypeConfiguration<TitleRating>
    {
        public void Configure(EntityTypeBuilder<TitleRating> builder)
        {
            builder
                .HasKey(cv => new { cv.UserId, cv.TitleId });

            builder
                .HasOne(cv => cv.User)
                .WithMany(u => u.TitleRatings)
                .HasForeignKey(cv => cv.UserId);

           builder
                .HasOne(cv => cv.Title)
                .WithMany(c => c.TitleRatings)
                .HasForeignKey(cv => cv.TitleId);
        }
    }
}