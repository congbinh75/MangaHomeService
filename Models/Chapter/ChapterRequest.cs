namespace MangaHomeService.Models
{
    public class ChapterRequest : BaseModel
    {
        public Chapter Chapter { get; set; }
        public Group Group { get; set; }
        public string Note { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
