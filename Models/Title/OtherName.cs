namespace MangaHomeService.Models
{
    public class OtherName : BaseEntity
    {
        public required string Name { get; set; }
        public required Language Language { get; set; }
    }
}
