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
    }

    public class ChapterRequestData : SubmitRequest
    {
        [Required]  
        public required string ChapterId { get; set; }

        [Required]
        public required string GroupId { get; set;}
    }

    public class GroupRequestData : SubmitRequest
    {
        [Required]
        public required string GroupId { get; set; }
    }

    public class TitleRequestData : SubmitRequest
    {
        [Required]
        public required string TitleId { get; set; }

        [Required]
        public required string GroupId { get; set; }
    }

    public class MemberRequestData : SubmitRequest
    {
        [Required]
        public required string MemberId { get; set;}

        [Required]
        public required string GroupId { get; set;}
    }

    public class ArtistRequestData : SubmitRequest
    {
        [Required]
        public required string PersonId { get; set; }
    }

    public class AuthorRequestData : SubmitRequest
    {
        [Required]
        public required string PersonId { get; set; }
    }
}
