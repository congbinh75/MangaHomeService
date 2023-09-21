namespace MangaHomeService.Models
{
    public class ThemeModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Title> Titles { get; set; }
    }
}
