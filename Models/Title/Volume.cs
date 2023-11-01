namespace MangaHomeService.Models
{
    public class Volume : BaseModel
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public Title Title { get; set; }
    }
}
