namespace MangaHomeService.Models
{
    public class ChapterTracking
    {
        public required User User { get; set; }
        public required Chapter Chapter { get; set; }
    }
}
