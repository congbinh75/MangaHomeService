namespace MangaHomeService.Models.Entities
{
    public class Theme : Tag
    {
        public required ICollection<Title> Titles { get; set; } = [];
    }
}