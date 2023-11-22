namespace MangaHomeService.Models
{
    public class CommentVote
    {
        public required User User { get; set; }
        public required Comment Comment { get; set; }
        public bool IsUpvote { get; set; }
    }
}
