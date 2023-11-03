namespace MangaHomeService.Models
{
    public class MemberRequest : BaseModel
    {
        public Group Group { get; set; }
        public Member Member { get; set; }
        public User SubmitUser { get; set; }
        public string Note { get; set; }
        public string ReviewUser { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
