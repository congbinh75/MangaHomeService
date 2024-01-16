using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Request : BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public required string SubmitNote { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string ReviewNote { get; set; } = string.Empty;

        [Required]
        public bool IsApproved { get; set; }

        [Required]
        public bool IsReviewed { get; set; }
    }
}

