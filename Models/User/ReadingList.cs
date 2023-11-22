namespace MangaHomeService.Models
{
    public class ReadingList : BaseModel
    {
        public User? User { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public List<Title>? Titles { get; set; }
    }
}
