namespace MangaHomeService.Models
{
    public class Group : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfilePicture { get; set; }
        public List<Member> Members { get; set; }
        public bool AllowMemberRequest { get; set; }
        public bool RequireChapterReview { get; set; }
    }
}
