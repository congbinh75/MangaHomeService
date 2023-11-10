namespace MangaHomeService.Models
{
    public class Comment : BaseModel
    {
        public string Content { get; set; }
        public int Vote { get; set; }
        public Chapter Chapter { get; set; }
        public Title Title { get; set; }
        public Group Group { get; set; }
    }
}
