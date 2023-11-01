namespace MangaHomeService.Models
{
    public class TitleRequest : BaseModel
    {
        public Title Title { get; set; }
        public User User { get; set; }
        public bool? IsApproved { get; set; }
    }
}
