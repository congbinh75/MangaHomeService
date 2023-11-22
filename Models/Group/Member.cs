namespace MangaHomeService.Models
{
    public class Member : BaseModel
    {
        public User? User { get; set; }
        public Group? Group { get; set; }
        public int Role { get; set; }
    }
}
