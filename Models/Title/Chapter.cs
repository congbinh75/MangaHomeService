namespace MangaHomeService.Models
{
    public class Chapter : BaseModel
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public List<Page> Pages { get; set; }
        public Group Group { get; set; }
        public List<Comment> Comments { get; set; }

        public Chapter() { }
        public Chapter(string number, string title, List<Page> pages, Group group, List<Comment> comments)
        {
            Number = number;
            Title = title;
            Pages = pages;
            Group = group;
            Comments = comments;
        }
    }
}
