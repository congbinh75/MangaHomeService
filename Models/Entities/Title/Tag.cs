namespace MangaHomeService.Models.Entities
{
    public class Tag : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required ICollection<OtherName> OtherNames { get; set; } = [];
    }

    public class Gerne : Tag 
    {
        public required ICollection<Title> Titles { get; set; } = [];
    }

    public class Theme : Tag 
    {
        public required ICollection<Title> Titles { get; set; } = [];
    }

    public class Demographic : Tag 
    {
        public required ICollection<Title> Titles { get; set; } = [];
    }
}
