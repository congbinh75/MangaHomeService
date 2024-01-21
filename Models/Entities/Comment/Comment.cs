using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Comment : BaseEntity
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required string Content { get; set; } = string.Empty;

        [Required]
        public required int Vote { get; set; } = 0;

        [Required]
        public required ICollection<CommentVote> CommentVotes { get; set; } = [];
    }
}
