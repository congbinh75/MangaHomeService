namespace MangaHomeService.Models
{
    public class TitleRating : BaseModel
    {
        public User User { get; set; }
        public int Rating { get; set; }
    }
}
