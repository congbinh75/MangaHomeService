namespace MangaHomeService.Models
{
    public class ChapterRequest : BaseModel
    {
        public Chapter Chapter { get; set; }
        public Group Group { get; set; }
        public string SubmitNote { get; set; }
        public string ReviewNote { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
