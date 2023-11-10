namespace MangaHomeService.Models
{
    public class GroupRequest : BaseModel
    {
        public Group Group { get; set; }
        public string Note { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
