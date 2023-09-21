namespace MangaHomeService.Models
{
    public class TitleRatingModel : BaseModel
    {
        public User User { get; set; }
        public int Rating { get; set; }
    }
}
