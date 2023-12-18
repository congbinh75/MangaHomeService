using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class TitleRating
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required Title Title { get; set; }

        [Required]
        public required string TitleId { get; set; }

        [Required]
        [Range(0, 5)]
        public required int Rating { get; set; } = 0;
    }
}
