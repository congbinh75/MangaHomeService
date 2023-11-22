namespace MangaHomeService.Models
{
    public class Chapter : BaseEntity
    {
        public required double Number { get; set; }
        public required Title Title { get; set; }
        public Volume? Volume { get; set; }
        public Language? Language { get; set; }
        public required ICollection<Page> Pages { get; set; } = [];
        public required Group Group { get; set; }
        public required ICollection<Comment> Comments { get; set; } = [];
        public required bool IsApproved { get; set; }
    }
}
