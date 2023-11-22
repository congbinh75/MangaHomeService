namespace MangaHomeService.Models
{
    public class Comment : BaseEntity
    {
        public required string Content { get; set; } = string.Empty;
        public required int Vote { get; set; } = 0;
    }

    public class ChapterComment : Comment
    {
        public required Chapter Chapter { get; set; }
    }

    public class TitleComment : Comment
    {
        public required Title Title { get; set; }
    }

    public class GroupComment : Comment
    {
        public required Group Group { get; set; }
    }
}
