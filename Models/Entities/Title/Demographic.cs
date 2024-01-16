namespace MangaHomeService.Models.Entities
{
    public class Demographic : Tag
    {
        public required ICollection<Title> Titles { get; set; } = [];
    }
}