namespace MangaHomeService.Models
{
    public class ChapterRequest : BaseModel
    {
        public Chapter Chapter { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        public bool? IsApproved { get; set; }
    }
}
