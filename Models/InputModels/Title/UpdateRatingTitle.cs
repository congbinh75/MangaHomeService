using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateRatingTilte
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [Range(1, 5)]
        public required int Rating { get; set; }

        public string? UserId { get; set; } = null;
    }
}
