namespace MangaHomeService.Models
{
    public class Group : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfilePicture { get; set; }
        public List<Member> Members { get; set; }

        public Group() { }
        public Group(string name, string description, string profilePicture, List<Member> members)
        {
            Name = name;
            Description = description;
            ProfilePicture = profilePicture;
            Members = members;
        }
    }
}
