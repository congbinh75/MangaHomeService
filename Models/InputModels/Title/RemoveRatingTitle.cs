using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class RemoveRatingTitle
    {
        [Required]
        public required string Id { get; set; }

        public string? UserId { get; set; } = null;
    }
}
