using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Report : BaseEntity
    {
        [Required]
        public required string Reason { get; set; }

        [Required]
        [MaxLength(1024)]
        public required string Note { get; set; } = string.Empty;

        [Required]
        public required bool IsReviewed { get; set; } = false;
    }

    public class UserReport : Report
    {
        [Required]
        public required User User { get; set; }
    }

    public class TitleReport : Report
    {
        [Required]
        public required Title Title { get; set; }
    }

    public class ChapterReport : Report
    {
        [Required]
        public required Chapter Chapter { get; set; }
    }

    public class GroupReport : Report
    {
        [Required]
        public required Group Group { get; set; }
    }

    public class CommentReport : Report
    {
        [Required]
        public required Comment Comment { get; set; }
    }
}
