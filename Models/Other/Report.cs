namespace MangaHomeService.Models
{
    public class Report : BaseModel
    {
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public string? IsReviewed { get; set; }
    }

    public class UserReport : Report
    {
        public User? User { get; set; }
    }

    public class TitleReport : Report
    {
        public Title? Title { get; set; }
    }

    public class ChapterReport : Report
    {
        public Chapter? Chapter { get; set; }
    }

    public class GroupReport : Report
    {
        public Group? Group { get; set; }
    }

    public class CommentReport : Report
    {
        public Comment? Comment { get; set; }
    }
}
