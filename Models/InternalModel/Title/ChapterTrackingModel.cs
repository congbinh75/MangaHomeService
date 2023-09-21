namespace MangaHomeService.Models
{
    public class ChapterTrackingModel : BaseModel
    {
        public User User { get; set; }
        public Chapter Chapter { get; set; }
    }
}
