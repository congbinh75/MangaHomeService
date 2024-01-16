using MangaHomeService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaHomeService.Models.Configurations
{
    public class CommentVoteConfiguration : IEntityTypeConfiguration<CommentVote>
    {
        public void Configure(EntityTypeBuilder<CommentVote> builder)
        {
            builder.HasKey(cv => new { cv.UserId, cv.CommentId });

            builder.HasOne(cv => cv.User)
                .WithMany(u => u.CommentVotes)
                .HasForeignKey(cv => cv.UserId);

            builder.HasOne(cv => cv.Comment)
                .WithMany(c => c.CommentVotes)
                .HasForeignKey(cv => cv.CommentId);
        }
    }
}