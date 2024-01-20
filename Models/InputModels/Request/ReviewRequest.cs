using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class ReviewRequest
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public required string ReviewNote { get; set; }

        [Required]
        public required bool IsApproved { get; set; }
    }
}
