namespace MangaHomeService.Models
{
    public class TitleRequest : BaseModel
    {
        public Title Title { get; set; }
        public User SubmitUser { get; set; }
        public Group Group { get; set; }
        public string Note { get; set; }
        public User ReviewUser { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
