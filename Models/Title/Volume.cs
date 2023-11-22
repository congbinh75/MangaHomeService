namespace MangaHomeService.Models
{
    public class Volume : BaseEntity
    {
        public required string Number { get; set; }
        public string? Name { get; set; } = string.Empty;
        public required Title Title { get; set; }
    }
}
