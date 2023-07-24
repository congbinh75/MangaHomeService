namespace MangaHomeService.Models
{
    public class Group : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfilePicture { get; set; }
        public User Leader { get; set; }
        public List<User> Members { get; set; }

        public Group() { }
        public Group(string name, string description, string profilePicture, User leader, List<User> members)
        {
            Name = name;
            Description = description;
            ProfilePicture = profilePicture;
            Leader = leader;
            Members = members;
        }
    }
}
