using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Request : BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public required string SubmitNote { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string ReviewNote { get; set; } = string.Empty;

        [Required]
        public bool IsApproved { get; set; }

        [Required]
        public bool IsReviewed { get; set; }
    }

    public class TitleRequest : Request
    {
        [Required]
        public required Title Title { get; set; }

        [Required]
        public required Group Group { get; set; }
    }

    public class MemberRequest : Request
    {
        [Required]
        public required Group Group { get; set; }

        [Required]
        public required Member Member { get; set; }
    }

    public class ChapterRequest : Request
    {
        [Required]
        public required Chapter Chapter { get; set; }

        [Required]
        public required Group Group { get; set; }
    }

    public class GroupRequest : Request
    {
        [Required]
        public required Group Group { get; set; }
    }

    public class ArtistRequest : Request
    {
        [Required]
        public required Person Artist { get; set; }
    }

    public class AuthorRequest : Request
    {
        [Required]
        public required Person Author { get; set; }
    }
}

