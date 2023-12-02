using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Comment : BaseEntity
    {
        [Required]
        public required string Content { get; set; } = string.Empty;

        [Required]
        public required int Vote { get; set; } = 0;
    }

    public class ChapterComment : Comment
    {
        [Required]
        public required Chapter Chapter { get; set; }
    }

    public class TitleComment : Comment
    {
        [Required]
        public required Title Title { get; set; }
    }

    public class GroupComment : Comment
    {
        [Required]
        public required Group Group { get; set; }
    }
}
