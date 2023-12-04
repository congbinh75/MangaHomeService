using MangaHomeService.Utils;
using System.ComponentModel.DataAnnotations;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Models.Entities
{
    public class Title : BaseEntity
    {
        [Required]
        [MaxLength(128)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public string? Artwork { get; set; } = string.Empty;

        [Required]
        public required ICollection<Person> Authors { get; set; } = [];

        [Required]
        public required ICollection<Person> Artists { get; set; } = [];

        [Required]
        public required ICollection<Tag> Demographics { get; set; } = [];

        [Required]
        [Range(0, 3)]
        public required TitleStatus Status { get; set; } = 0;

        [Required]
        [Range(0, 5)]
        public required double Rating { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue)]
        public required int RatingVotes { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue)]
        public required int Views { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue)]
        public required int Bookmarks { get; set; } = 0;

        [Required]
        public required ICollection<OtherName> OtherNames { get; set; } = [];

        [Required]
        public Language? OriginalLanguage { get; set; }

        [Required]
        public required ICollection<Tag> Gernes { get; set; } = [];

        [Required]
        public required ICollection<Tag> Themes { get; set; } = [];

        [Required]
        public required ICollection<Chapter> Chapters { get; set; } = [];

        [Required]
        public required ICollection<Comment> Comments { get; set; } = [];

        [Required]
        public required bool IsApproved { get; set; }

        public void CheckUploadConditions()
        {
            if (!IsApproved)
            {
                throw new NotApprovedException(Name ?? "");
            }
        }
    }
}
