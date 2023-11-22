namespace MangaHomeService.Models
{
    public class Report : BaseEntity
    {
        public required string Reason { get; set; }
        public required string Note { get; set; } = string.Empty;
        public required bool IsReviewed { get; set; } = false;
    }

    public class UserReport : Report
    {
        public required User User { get; set; }
    }

    public class TitleReport : Report
    {
        public required Title Title { get; set; }
    }

    public class ChapterReport : Report
    {
        public required Chapter Chapter { get; set; }
    }

    public class GroupReport : Report
    {
        public required Group Group { get; set; }
    }

    public class CommentReport : Report
    {
        public required Comment Comment { get; set; }
    }
}
