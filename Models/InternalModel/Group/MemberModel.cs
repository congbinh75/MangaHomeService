namespace MangaHomeService.Models
{
    public class MemberModel : BaseModel
    {
        public User User { get; set; }
        public Group Group { get; set; }
        public bool IsLeader { get; set; }
    }
}
