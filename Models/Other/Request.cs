namespace MangaHomeService.Models
{
    public class Request : BaseEntity
    {
        public string SubmitNote { get; set; } = string.Empty;
        public string ReviewNote { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }

    public class TitleRequest : Request
    {
        public required Title Title { get; set; }
        public required Group Group { get; set; }
    }

    public class MemberRequest : Request
    {
        public required Group Group { get; set; }
        public required Member Member { get; set; }
    }

    public class ChapterRequest : Request
    {
        public required Chapter Chapter { get; set; }
        public required Group Group { get; set; }
    }

    public class GroupRequest : Request
    {
        public required Group Group { get; set; }
    }
}

