namespace MangaHomeService.Models
{
    public class Tag : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required int Type { get; set; }
        public required ICollection<Title> Titles { get; set; } = [];
        public required ICollection<OtherName> OtherNames { get; set; } = [];
    }
}
