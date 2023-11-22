namespace MangaHomeService.Models
{
    public class TitleRating
    {
        public required User User { get; set; }
        public required Title Title { get; set; }
        public required int Rating { get; set; } = 0;
    }
}
