namespace MangaHomeService.Models
{
    public class Member : BaseModel
    {
        public User User { get; set; }
        public Group Group { get; set; }
        public bool IsLeader { get; set; }
        public Member() { }
        public Member(User user, Group group, bool isLeader) 
        {
            User = user;
            Group = group;
            IsLeader = isLeader;
        }
    }
}
