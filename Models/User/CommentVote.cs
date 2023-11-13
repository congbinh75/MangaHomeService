namespace MangaHomeService.Models
{
    public class CommentVote
    {
        public User User { get; set; }
        public Comment Comment { get; set; }
        public bool IsUpvote { get; set; }
    }
}
