namespace MangaHomeService.Models
{
    public class Page : BaseModel
    {
        public Chapter Chapter { get; set; }
        public int Number { get; set; }
        public string File { get; set; }
        
        public Page() { }
        public Page(Chapter chapter, int number, string file)
        {
            Chapter = chapter;
            Number = number;
            File = file;
        }
    }
}
