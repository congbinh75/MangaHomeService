namespace MangaHomeService.Models
{
    public class Gerne : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Title> Titles { get; set; }
        public Gerne() { }
        public Gerne(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
