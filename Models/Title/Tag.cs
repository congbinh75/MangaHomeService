namespace MangaHomeService.Models
{
    public class Tag : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required ICollection<Title> Titles { get; set; } = [];
        public required ICollection<OtherName> OtherNames { get; set; } = [];
    }

    public class Gerne : Tag { }
    public class Theme : Tag { }
    public class Demographic : Tag { }
}
