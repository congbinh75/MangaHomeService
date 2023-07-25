namespace MangaHomeService.Models
{
    public class ReadingList : BaseModel
    {
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Title> Titles { get; set; }

        public ReadingList() { }
        public ReadingList(User user, string name, string description)
        {
            User = user;
            Name = name;
            Description = description;
        }
    }
}
