namespace MangaHomeService.Models
{
    public class Person : BaseEntity
    {
        public required string Name { get; set; }
        public string? Image { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public required ICollection<Title> AuthoredTitles { get; set; } = [];
        public required ICollection<Title> IllustratedTitles { get; set; } = [];
        public bool IsApproved { get; set; }
    }
}
