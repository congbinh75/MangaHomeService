using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class SubmitRequest
    {
        [Required]
        public required string RequestType { get; set; }

        [Required]
        [MaxLength(256)]
        public required string SubmitNote { get; set; }

        [Required]
        public required object Data { get; set; }
    }

    public class ChapterRequestData
    {
        [Required]  
        public required string ChapterId { get; set; }

        [Required]
        public required string GroupId { get; set;}
    }

    public class GroupRequestData
    {
        [Required]
        public required string GroupId { get; set; }
    }

    public class TitleRequestData
    {
        [Required]
        public required string TitleId { get; set; }

        [Required]
        public required string GroupId { get; set; }
    }

    public class MemberRequestData
    {
        [Required]
        public required string MemberId { get; set;}

        [Required]
        public required string GroupId { get; set;}
    }

    public class ArtistRequestData
    {
        [Required]
        public required string PersonId { get; set; }
    }

    public class AuthorRequestData
    {
        [Required]
        public required string PersonId { get; set; }
    }
}
