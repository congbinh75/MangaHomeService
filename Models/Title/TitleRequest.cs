namespace MangaHomeService.Models
{
    public class TitleRequest : BaseModel
    {
        public Title Title { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
