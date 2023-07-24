namespace MangaHomeService.Models.Group
{
    public class Group
    {
        public string Name { get; set; }
        public User Leader { get; set; }
        public List<User> Members { get; set; }
    }
}
