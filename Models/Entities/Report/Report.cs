using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Report : BaseEntity
    {
        [Required]
        public required string Reason { get; set; }

        [Required]
        [MaxLength(1024)]
        public required string Note { get; set; } = string.Empty;

        [Required]
        public required bool IsReviewed { get; set; } = false;
    }
}
