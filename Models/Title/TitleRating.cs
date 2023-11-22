namespace MangaHomeService.Models
{
    public class TitleRating
    {
        public User? User { get; set; }
        public Title? Title { get; set; }
        public int Rating { get; set; }
    }
}
