namespace MangaHomeService.Models
{
    public class TitleRating : BaseModel
    {
        public User User { get; set; }
        public int Rating { get; set; }

        public TitleRating() { }
        public TitleRating(User user, int rating) 
        {
            User = user;
            Rating = rating;
        }
    }
}
