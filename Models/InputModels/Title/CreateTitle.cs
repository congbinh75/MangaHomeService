using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class CreateTitle
    {
        [Required]
        [MaxLength(128)]
        public required string Name { get; set; }

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
        public required ICollection<string> OtherNamesIds { get; set; } = [];

        [Required]
        public string? OriginalLanguageId { get; set; }

        [Required]
        public required ICollection<string> GernesIds { get; set; } = [];

        [Required]
        public required ICollection<string> ThemesIds { get; set; } = [];

        [Required]
        public required ICollection<string> DemographicsIds { get; set; } = [];
    }
}
