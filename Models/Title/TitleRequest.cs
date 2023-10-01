namespace MangaHomeService.Models
{
    public class TitleRequest : BaseModel
    {
        public Title Title { get; set; }
        public User User { get; set; }
        public bool? IsApproved { get; set; }
        
        public TitleRequest() { }
        public TitleRequest(Title title, User user, bool isApproved)
        {
            Title = title;
            User = user;
            IsApproved = isApproved;
        }
    }
}
