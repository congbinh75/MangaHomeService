namespace MangaHomeService.Models
{
    public class Chapter : BaseModel
    {
        public double Number { get; set; }
        public Title Title { get; set; }
        public Volume Volume { get; set; }
        public Language Language { get; set; }
        public List<Page> Pages { get; set; }
        public Group Group { get; set; }
        public List<Comment> Comments { get; set; }
        public bool IsApproved { get; set; }
    }
}
