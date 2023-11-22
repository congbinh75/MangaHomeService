namespace MangaHomeService.Models
{
    public class Page : BaseEntity
    {
        public required Chapter Chapter { get; set; }
        public required int Number { get; set; }
        public required string File { get; set; }
    }
}
