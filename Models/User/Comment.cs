namespace MangaHomeService.Models
{
    public class Comment : BaseModel
    {
        public string Content { get; set; }
        public User User { get; set; }
        public int Vote { get; set; }
        public Chapter? Chapter { get; set; }
        public Title? Title { get; set; }
        public Group? Group { get; set; }

        public Comment() { }
        public Comment(string content, User user, int vote = 0, Chapter? chapter = null, Title? title = null, Group? group = null)
        {
            Content = content;
            User = user;
            Vote = vote;
            Chapter = chapter;
            Title = title;
            Group = group;
        }
    }
}
