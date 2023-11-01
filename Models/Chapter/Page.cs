namespace MangaHomeService.Models
{
    public class Page : BaseModel
    {
        public Chapter Chapter { get; set; }
        public int Number { get; set; }
        public string File { get; set; }
    }
}
