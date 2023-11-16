namespace MangaHomeService.Models
{
    public class BaseModel
    {
        public string Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public User CreatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public User UpdatedBy { get; set; }
    }
}
