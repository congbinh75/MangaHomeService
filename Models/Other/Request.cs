namespace MangaHomeService.Models
{
    public class Request : BaseModel
    {
        public string SubmitNote { get; set; }
        public string ReviewNote { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }

    public class TitleRequest : Request
    {
        public Title Title { get; set; }
        public Group Group { get; set; }
    }

    public class MemberRequest : Request
    {
        public Group Group { get; set; }
        public Member Member { get; set; }
    }

    public class ChapterRequest : Request
    {
        public Chapter Chapter { get; set; }
        public Group Group { get; set; }
    }

    public class GroupRequest : Request
    {
        public Group Group { get; set; }
    }
}

