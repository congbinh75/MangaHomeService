namespace MangaHomeService.Models.Entities
{
    public class Gerne : Tag
    {
        public required ICollection<Title> Titles { get; set; } = [];
    }
}