namespace MangaHomeService.Models.Entities
{
    public class Tag : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required ICollection<OtherName> OtherNames { get; set; } = [];
    }
}
