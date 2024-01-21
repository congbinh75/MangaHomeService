using MangaHomeService.Utils;
using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public required string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(16)]
        public required string Password { get; set; }

        [Required]
        public required bool IsEmailConfirmed { get; set; } = false;

        [Required]
        public string? ProfilePicture { get; set; } = string.Empty;

        [Required]
        [Range(0, 2)]
        public required int Role { get; set; } = (int)Enums.Role.User;

        [Required]
        public required byte[] Salt { get; set; } = [];

        [Required]
        public ICollection<Member> Members { get; set; } = [];

        [Required]
        public ICollection<Title> UpdateFeed { get; set; } = [];

        [Required]
        public required bool IsBanned { get; set; } = false;

        public string? EmailConfirmationToken { get; set; }

        [Required]
        public required ICollection<Chapter> ChapterTrackings { get; set; } = [];

        [Required]
        public required ICollection<Comment> Comments { get; set; } = [];

        [Required]
        public required ICollection<CommentVote> CommentVotes { get; set; } = [];

        [Required]
        public required ICollection<TitleRating> TitleRatings { get; set; } = [];
    }
}
