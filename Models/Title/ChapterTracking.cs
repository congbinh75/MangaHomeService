namespace MangaHomeService.Models
{
    public class ChapterTracking : BaseModel
    {
        public User User { get; set; }
        public Chapter Chapter { get; set; }
        
        public ChapterTracking() { }
        public ChapterTracking(User user, Chapter chapter)
        {
            User = user;
            Chapter = chapter;
        }
    }
}
