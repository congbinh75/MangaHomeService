namespace MangaHomeService.Models
{
    public class Member : BaseModel
    {
        public User User { get; set; }
        public Group Group { get; set; }
        public GroupRole Role { get; set; }
        public Member() { }
        public Member(User user, Group group, GroupRole role) 
        {
            User = user;
            Group = group;
            Role = role;
        }
    }
}
