using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class CommentVote
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required Comment Comment { get; set; }

        [Required]
        public required string CommentId { get; set; }

        [Required]
        public bool IsUpvote { get; set; }
    }
}
