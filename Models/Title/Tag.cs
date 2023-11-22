namespace MangaHomeService.Models
{
    public class Tag : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Type { get; set; }
        public ICollection<Title>? Titles { get; set; }
        public ICollection<OtherName>? OtherNames { get; set; }
    }
}
