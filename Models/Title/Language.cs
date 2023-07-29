namespace MangaHomeService.Models
{
    public class Language : BaseModel
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public Language() { }
        public Language(string name, string logo)
        {
            Name = name;
            Logo = logo;
        }
    }
}
