namespace MangaHomeService.Models
{
    public class ReadingList : BaseEntity
    {
        public required User User { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required bool IsPublic { get; set; } = false;
        public List<Title> Titles { get; set; } = [];
    }
}
