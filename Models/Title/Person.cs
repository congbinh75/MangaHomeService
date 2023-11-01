namespace MangaHomeService.Models
{
    public class Person : BaseModel
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
    }
}
