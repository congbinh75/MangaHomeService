namespace MangaHomeService.Models
{
    public class Comment : BaseModel
    {
        public string Content { get; set; }
        public int Vote { get; set; }
    }

    public class ChapterComment : Comment
    {
        public Chapter Chapter { get; set; }
    }

    public class TitleComment : Comment
    {
        public Title Title {  get; set; }
    }

    public class GroupComment : Comment
    {
        public Group Group { get; set; }
    }
}
