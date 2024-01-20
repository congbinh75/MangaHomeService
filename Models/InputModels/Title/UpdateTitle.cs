using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateTitle
    {
        [Required]
        public required string Id { get; set; }

        [MaxLength(128)]
        public string? Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public IFormFile? Artwork { get; set; } = null;

        [Required]
        public required ICollection<string> AuthorsIds { get; set; } = [];

        [Required]
        public required ICollection<string> ArtistsIds { get; set; } = [];

        [Required]
        [Range(0, 3)]
        public required int Status { get; set; } = 0;

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
        public required ICollection<string> OtherNamesIds { get; set; } = [];

        [Required]
        public string? OriginalLanguageId { get; set; }

        [Required]
        public required ICollection<string> GernesIds { get; set; } = [];

        [Required]
        public required ICollection<string> ThemesIds { get; set; } = [];

        [Required]
        public required ICollection<string> DemographicsIds { get; set; } = [];

        [Required]
        public required ICollection<string> ChaptersIds { get; set; } = [];

        [Required]
        public required ICollection<string> CommentsIds { get; set; } = [];

        [Required]
        public required ICollection<string> TitleRatingsUsersIds { get; set; } = [];

        [Required]
        public required bool IsApproved { get; set; }
    }
}
